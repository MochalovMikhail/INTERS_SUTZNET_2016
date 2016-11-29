using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using NLog;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp;
using SUTZ_2.Module.BO.Documents;
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using System.Diagnostics;

namespace SUTZ_2.Module.BO.References.Mobile.Modules
{
    /* Класс предназначен для создания и удаления заданий на работу для моб.сутз
     * по всем видам документов
     */ 
    [NonPersistent]
    //[VisibleInDetailView(false), VisibleInListView(false)]
    public class ControlWorkUnitsByDocAndJobe
    {        
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        private IObjectSpace objectSpace {get; set;}
        private BaseDocument currentDoc {get; set;}
        private DocJobesProperties currentJobe { get; set; }
        private enTypesOfMobileStates oldType{get; set;}// старое разрешение на документ, до момента загрузки
        private enTypesOfMobileStates newType{get; set;}// новое разрешение на документ, загружаемое
        private readonly bool createObSpaceAndCommitTrans;// флаг, определяющий необходимость создания внутреннего ObjSpace и выполнения транзакции
        public Action<String, string, Exception> sendMessage { get; set; }


        /* Конструктор для вызова из модуля загрузки из 7.7
         * Передаваемые параметры:
         *              "objSpace" - сессия, в которой выполняется модуль загрузки. Если сессия передана, то финальная транзакция 
         *              будет выполняться в модуле загрузки, а не здесь.
         */
        public ControlWorkUnitsByDocAndJobe(IObjectSpace objSpace)
        {
            //objectSpace = currentSessionSettings.ObjXafApp.CreateObjectSpace();
            Guard.ArgumentNotNull(objSpace, "objSpace");
            objectSpace = objSpace;
 
            createObSpaceAndCommitTrans = false;// результаты работы будут сохраняться в вызывающем модуле.
        }

        /* Основной метод класса: Конструктор для вызова из модуля загрузки из 7.7
         * Передаваемые параметры:
         *              "objSpace" - сессия, в которой выполняется модуль загрузки. Если сессия передана, то финальная транзакция 
         *              будет выполняться в модуле загрузки, а не здесь.
         *              "baseDoc" - ссылка на обрабатываемый документ;
         *              "docJobe" - ссылка на элемент таблицы docJobeProperties;
         *              "oldType" - старое значение перечисления
         *              "newType" - новое значение перечисления
         */

        public bool proccessDoc(DocJobesProperties docJobe, enTypesOfMobileStates oldType_, enTypesOfMobileStates newType_)
        {
            bool resultProcess = false;
            Guard.ArgumentNotNull(docJobe, "docJobe");
            Guard.ArgumentNotNull(oldType, "oldType");
            Guard.ArgumentNotNull(newType, "newType");
            Guard.ArgumentNotNull(docJobe.BaseDoc, "docJobe.BaseDoc");

            currentDoc = docJobe.BaseDoc;
            currentJobe = docJobe;
            oldType = oldType_;
            newType = newType_;

            if (currentDoc.DocumentType==enDocumentType.СпецификацияПеремещения)
            {
                resultProcess = processDocPeremeshenie();
            }
            else if (currentDoc.DocumentType == enDocumentType.СпецификацияРасхода)
            {
                resultProcess = processDocSpecRashoda();
            }
            return resultProcess;
        }

        /* Метод разбивает табличную часть документа Спец.Перемещения на паллеты с учетом точки передачи, и устанавливает подходящий тип работы
         */
        private List<DocTableByPallets> getPalletsFromDocTableSpecPeremesh(BaseDocument tekDoc)
        {
            DocSpecPeremeshenie specDoc = tekDoc as DocSpecPeremeshenie;
            if (specDoc==null)
            {
                return null;
            }

            // 2. создание списка с возвращаемым значением:
            List<DocTableByPallets> lisDocTableByPallets = new List<DocTableByPallets>();
            XPQuery<JobTypes> queryJobTypes = new XPQuery<JobTypes>(objectSpace.Session());

            // 2.1 определение вида работы для точки передачи
            JobTypes elementJobTypeExchPointFrom = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.ПеремещениеВТочкуПередачи && c.IsMarkDeleted != true select c).FirstOrDefault<JobTypes>();

            foreach (DocSpecPeremeshGoods item in specDoc.DocListOfGoods)
            {
                // 2.1 Если выбрана точка передачи
                if (item.ExchangePoint!=null)
                {
                    // 2.1.1 перемещение из отправителя в точку передачи
                    var itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom == item.TakeFrom.PalletCode && o.strPalletCodePutTo == item.ExchangePoint.PalletCode);
                    if (itemfinded == null)
                    {
                        itemfinded = new DocTableByPallets
                        {
                            strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                            strPalletCodePutTo = item.ExchangePoint.PalletCode,
                            typeOfWork = enTypeOfWorks.ПеремещениеВТочкуПередачи,
                            currentJobeType = elementJobTypeExchPointFrom
                        };
                        itemfinded.listOfDocTable.Add(item.idd);
                        lisDocTableByPallets.Add(itemfinded);
                    }
                    else if (!itemfinded.listOfDocTable.Contains(item.idd))
                    {
                        itemfinded.listOfDocTable.Add(item.idd);
                    }

                    // 2.1.2 перемещение из точки передачи в получателя
                    JobTypes elementJobTypeExchPointTo = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.ПеремещениеИзТочкиПередачи && c.IsMarkDeleted != true select c).FirstOrDefault<JobTypes>();

                    itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom == item.ExchangePoint.PalletCode && o.strPalletCodePutTo == item.PutTo.PalletCode);
                    if (itemfinded == null)
                    {
                        itemfinded = new DocTableByPallets
                        {
                            strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                            strPalletCodePutTo = item.ExchangePoint.PalletCode,
                            typeOfWork = enTypeOfWorks.ПеремещениеИзТочкиПередачи,
                            currentJobeType = elementJobTypeExchPointTo
                        };
                        itemfinded.listOfDocTable.Add(item.idd);
                        lisDocTableByPallets.Add(itemfinded);
                    }
                    else if (!itemfinded.listOfDocTable.Contains(item.idd))
                    {
                        itemfinded.listOfDocTable.Add(item.idd);
                    }
                }
                else//2.2 точка передачи не выбрана
                {
                    JobTypes elementJobTypeStandardMove = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.Перемещение && c.IsMarkDeleted != true select c).FirstOrDefault<JobTypes>();
                    var itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom==item.TakeFrom.PalletCode && o.strPalletCodePutTo==item.PutTo.PalletCode);
                    if (itemfinded==null)
                    {
                        itemfinded = new DocTableByPallets
                        {
                            strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                            strPalletCodePutTo    = item.PutTo.PalletCode,
                            typeOfWork            = enTypeOfWorks.Перемещение,
                            currentJobeType       = elementJobTypeStandardMove
                        };
                        itemfinded.listOfDocTable.Add(item.idd);
                        lisDocTableByPallets.Add(itemfinded);
                    }
                    else if (!itemfinded.listOfDocTable.Contains(item.idd))
                    {
                        itemfinded.listOfDocTable.Add(item.idd);
                    }
                }
            }
            return lisDocTableByPallets;
        }

        /* Метод привязывает существующие в системе единицы работы к строкам документа
         *      docTableByPallets - список паллет документа
         *      listAllUnits -  список единиц работы
         */
        private void linkMobileWorkUnitTodocTablePallets(List<DocTableByPallets> docTableByPallets, IQueryable<MobileWorkUnit> listAllUnits)
        {
            // 1. проверка количества элементов в единицах работы
            if (listAllUnits.Count()==0)
            {
                return;
            }

            // 2. преобразование списка единиц работы в список
            //List<MobileWorkUnit> allUnits = listAllUnits.ToList<MobileWorkUnit>();            

            // 2. цикл по таблице паллет документа:
            foreach (var item in docTableByPallets)
            {          
                // 1.1 нужно найти задание с точно таким же набором idd строк документа и типом работы
                //List<int> docIdsByPallets = item.listOfDocTable.Select(o => o.idd).OrderBy(o=>o).ToList();

                foreach (var mwUnit in listAllUnits)
                {
                    var mwUnitMobileWorkDocRowsCollectionToList = (from  s in mwUnit.MobileWorkDocRowsCollection select s.BaseDocRowId).ToList<int>();
                    if (mwUnitMobileWorkDocRowsCollectionToList.SequenceEqual(item.listOfDocTable))
                    {
                        item.mobileWorkUnit = mwUnit;
                        break;
                    }
                }

                //IEnumerable<int> docIdsByPallets = item.listOfDocTable.Select(o => o.idd).ToList().AsEnumerable();
                
                //var findUnit = (from s in allUnits where s.MobileWorkDocRowsCollection.Select(oo => oo.idd).AsEnumerable<int>().SequenceEqual(docIdsByPallets) select s).FirstOrDefault<MobileWorkUnit>();
                ////var findUnit = listAllUnits.FirstOrDefault<MobileWorkUnit>(s => s.MobileWorkDocRowsCollection.Select(a => a.idd).SequenceEqual(docIdsByPallets));

                //2.1 получение массива идентификаторов строк документов:
                //try
                //{
                //    var findUnit = listAllUnits.FirstOrDefault<MobileWorkUnit>(s => s.MobileWorkDocRowsCollection.Select(a => a.idd).OrderBy(o => o).Where(a => a. SequenceEqual(docIdsByPallets));
                //    //var err = from s in listAllUnits select s.MobileWorkDocRowsCollection
                //    if (findUnit != null)
                //    {
                //        item.mobileWorkUnit = findUnit;
                //    }
                //}
                //catch (System.Exception ex)
                //{
                //    logger.Error(ex,"ошибка выполнения запроса !", listAllUnits);
                //} 
            }
        }

        /* Метод создает единицы работы по документу и виду работы или меняет существующие задания.
         */
        private bool processDocPeremeshenie()
        {
            // 1. поиск всех существующих не помеченных на удаление заданий по документу и виду работы
            XPQuery<MobileWorkUnit> queryRealWorkUnits = new XPQuery<MobileWorkUnit>(objectSpace.Session());

            var listRealUnits = from o in queryRealWorkUnits 
                                where 
                                o.BaseDoc.idd==currentDoc.idd &&
                                o.JobType.idd==currentJobe.idd &&
                                o.IsMarkDeleted != true
                                select o;

            // 2. разделение табличной части документа на паллеты:
            List<DocTableByPallets> docTableByPallets = getPalletsFromDocTableSpecPeremesh(currentDoc);
            if ((docTableByPallets==null)||(docTableByPallets.Count==0))
            {
                return false;
            }

            // 3. привязка к табличной части существующих заданий
            linkMobileWorkUnitTodocTablePallets(docTableByPallets, listRealUnits);

            // 4. попытка изменения статуса существующего задания, только если оно не выполнено или не в работе.
            foreach (var item in docTableByPallets)
            {
                if (item.mobileWorkUnit != null)
                {
                    if (newType == enTypesOfMobileStates.ВПроцессеОтзыва)
                    {
                        item.mobileWorkUnit.TypeOfMobileState = enTypesOfMobileStates.ОтозванИзРаботы;
                    }
                    else if (newType == enTypesOfMobileStates.ВПроцессеОтправки)
                    {
                        item.mobileWorkUnit.TypeOfMobileState = enTypesOfMobileStates.ДоступенДляРаботы;
                    }
                    else if (item.mobileWorkUnit.TypeOfMobileState!=newType)
                    {
                        item.mobileWorkUnit.TypeOfMobileState = newType;                        
                    }
                    item.mobileWorkUnit.Save();
                }
            }

            // 5. создание новых заданий, которых еще нет в БД:            
            Users current_User = (Users)SecuritySystem.CurrentUser;
            Users currentUser = objectSpace.FindObject<Users>(new BinaryOperator("idd", current_User.idd, BinaryOperatorType.Equal));
            Delimeters delimeter = objectSpace.FindObject<Delimeters>(new BinaryOperator("DelimeterID", current_User.DefaultDelimeter.DelimeterID, BinaryOperatorType.Equal), true);

            XPQuery<DocSpecPeremeshGoods> queryDocTable = new XPQuery<DocSpecPeremeshGoods>(objectSpace.Session());

            foreach (var item in docTableByPallets)
            {
                if (item.mobileWorkUnit == null)
                {

                    //Nullable<Decimal> quantity_Plan = (from o in item.listOfDocTable select o.QuantityOrig).Sum();
                    var quantity_Plan = queryDocTable.Where(o => item.listOfDocTable.Contains(o.idd)).Sum(o => o.QuantityOrig);
                    //Decimal quantityPlan = quantity_Plan ?? 0;

                    MobileWorkUnit newMWUnit = new MobileWorkUnit(objectSpace.Session())
                    {
                        BaseDoc = currentDoc,
                        JobType = item.currentJobeType,
                        AssignedUser = null,
                        TypeOfMobileState = enTypesOfMobileStates.ДоступенДляРаботы,
                        SendedUser = currentUser,
                        Delimeter = delimeter,
                        dTimeInsertUnit = DateTime.Now,
                        TypeOfUnitWork = enTypesOfUnitWork.НеОпределен,
                        Barcode = "",                       
                        QuantityPlan = quantity_Plan,
                        QuantityFact = 0
                    };
                    newMWUnit.Save();

                    // добавление строк документа, по которым будет выполняться размещение:
                    foreach (var itemRow in item.listOfDocTable)
                    {
                        DocSpecPeremeshGoods itemDoc = objectSpace.Session().GetObjectByKey<DocSpecPeremeshGoods>(itemRow, true);
                        if (itemDoc==null)
                        {
                            logger.Warn("processDocPeremeshenie: ошибка поиска строки документа по idd. документ: {0},iddoc={1} idd строки={2}", currentDoc.ToString(), currentDoc.idd, itemRow);
                            continue;
                        }

                        MobileWorkDocRows newDocRow = new MobileWorkDocRows(objectSpace.Session()) 
                        { 
                            BaseDocRowId = itemDoc.idd, 
                            BaseDocRowGUID = itemDoc.DocLineUID, 
                            idGUID = Guid.NewGuid(), 
                            Delimeter = delimeter, 
                            LineNo = itemDoc.LineNo 
                        };
                        newDocRow.Save();
                        newMWUnit.MobileWorkDocRowsCollection.Add(newDocRow);    
                    }                    
                }
            }

            // 6. удаление существующих не выполненных заданий, которые не удалось привязать к табличной части документа
            var iddRealUnits = from o in docTableByPallets where o.mobileWorkUnit!=null select o.mobileWorkUnit.idd;

            var deletedUnits = from o in listRealUnits 
                               where 
                               //o.TypeOfMobileState==enTypesOfMobileStates.ДоступенДляРаботы
                                !iddRealUnits.Contains(o.idd)
                               select o;
            foreach (var item in deletedUnits)
            {
                item.Delete();
            }

            objectSpace.CommitChanges();

            return true;
        }


        #region Обработчик документа Спецификация расхода
        /* Метод разбивает табличную часть документа Спец.Перемещения на паллеты с учетом точки передачи, и устанавливает подходящий тип работы
        */
        private List<DocTableByPallets> getPalletsFromDocTableSpecRashoda(BaseDocument tekDoc)
        {
            DocSpecRashoda specDoc = tekDoc as DocSpecRashoda;
            if (specDoc == null)
            {
                return null;
            }

            // 2. создание списка с возвращаемым значением:
            List<DocTableByPallets> lisDocTableByPallets = new List<DocTableByPallets>();
            XPQuery<JobTypes> queryJobTypes = new XPQuery<JobTypes>(objectSpace.Session());

            // 2.1 определение вида работы для точки передачи

            //Остановился: 
            //3. если выбран код размещения в кладовой отгрузки, создание заданий.

            JobTypes elementJobTypeExchPointFrom = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.ОтборВТочкуПередачи && c.IsMarkDeleted != true select c).FirstOrDefault<JobTypes>();
            JobTypes elementJobTypeExchPointTo = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.ОтборИзТочкиПередачи && c.IsMarkDeleted != true select c).FirstOrDefault<JobTypes>();
            JobTypes elementJobTypeStandardMove = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.ОтборТовара && c.IsMarkDeleted != true select c).FirstOrDefault<JobTypes>();

            if (elementJobTypeExchPointFrom==null)
            {
                sendMessage(String.Format("ControlWorkUnitsByDocAndJobe:getPalletsFromDocTableSpecRashoda: ошибка поиска операции с типом 'Отбор из точки передачи', задания для моб.сутз не созданы! документ: {0}", tekDoc), "Warning", null);
                return lisDocTableByPallets;
            }
            if (elementJobTypeExchPointTo == null)
            {
                sendMessage(String.Format("ControlWorkUnitsByDocAndJobe:getPalletsFromDocTableSpecRashoda: ошибка поиска операции с типом 'Отбор в точку передачи', задания для моб.сутз не созданы! документ: {0}", tekDoc), "Warning", null);
                return lisDocTableByPallets;
            }
            if (elementJobTypeStandardMove == null)
            {
                sendMessage(String.Format("ControlWorkUnitsByDocAndJobe:getPalletsFromDocTableSpecRashoda: ошибка поиска операции с типом 'Отбор товара', задания для моб.сутз не созданы! документ: {0}", tekDoc), "Warning", null);
                return lisDocTableByPallets;
            }

            foreach (DocSpecRashodaGoods item in specDoc.DocListOfGoods)
            {
                // 2.1 Если выбрана точка передачи
                if (item.ExchangePoint != null)
                {
                    // 2.1.1 перемещение из отправителя в точку передачи
                    var itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom == item.TakeFrom.PalletCode && o.strPalletCodePutTo == item.ExchangePoint.PalletCode);
                    if (itemfinded == null)
                    {
                        itemfinded = new DocTableByPallets
                        {
                            strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                            strPalletCodePutTo = item.ExchangePoint.PalletCode,
                            typeOfWork = enTypeOfWorks.ОтборВТочкуПередачи,
                            currentJobeType = elementJobTypeExchPointFrom
                        };
                        itemfinded.listOfDocTable.Add(item.idd);
                        lisDocTableByPallets.Add(itemfinded);
                    }
                    else if (!itemfinded.listOfDocTable.Contains(item.idd))
                    {
                        itemfinded.listOfDocTable.Add(item.idd);
                    }

                    // 2.1.2 перемещение из точки передачи в кладовую отгрузки:
                    if (item.PutTo!=null)
                    {
                        itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom == item.ExchangePoint.PalletCode && o.strPalletCodePutTo == item.PutTo.PalletCode);
                        if (itemfinded == null)
                        {
                            itemfinded = new DocTableByPallets
                            {
                                strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                                strPalletCodePutTo = item.ExchangePoint.PalletCode,
                                typeOfWork = enTypeOfWorks.ОтборИзТочкиПередачи,
                                currentJobeType = elementJobTypeExchPointTo
                            };
                            itemfinded.listOfDocTable.Add(item.idd);
                            lisDocTableByPallets.Add(itemfinded);
                        }
                        else if (!itemfinded.listOfDocTable.Contains(item.idd))
                        {
                            itemfinded.listOfDocTable.Add(item.idd);
                        }
                    }                    
                }
                else//2.2 точка передачи не выбрана
                {
                    if (item.PutTo != null)
                    {
                        var itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom == item.TakeFrom.PalletCode && o.strPalletCodePutTo == item.PutTo.PalletCode);
                        if (itemfinded == null)
                        {
                            itemfinded = new DocTableByPallets
                            {
                                strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                                strPalletCodePutTo = item.PutTo.PalletCode,
                                typeOfWork = enTypeOfWorks.ОтборТовара,
                                currentJobeType = elementJobTypeStandardMove
                            };
                            itemfinded.listOfDocTable.Add(item.idd);
                            lisDocTableByPallets.Add(itemfinded);
                        }
                        else if (!itemfinded.listOfDocTable.Contains(item.idd))
                        {
                            itemfinded.listOfDocTable.Add(item.idd);
                        }
                    }
                    else// положитьВ не выбран
                    {
                        var itemfinded = lisDocTableByPallets.Find(o => o.strPalletCodeTakeFrom == item.TakeFrom.PalletCode);
                        if (itemfinded == null)
                        {
                            itemfinded = new DocTableByPallets
                            {
                                strPalletCodeTakeFrom = item.TakeFrom.PalletCode,
                                typeOfWork = enTypeOfWorks.ОтборТовара,
                                currentJobeType = elementJobTypeStandardMove
                            };
                            itemfinded.listOfDocTable.Add(item.idd);
                            lisDocTableByPallets.Add(itemfinded);
                        }
                        else if (!itemfinded.listOfDocTable.Contains(item.idd))
                        {
                            itemfinded.listOfDocTable.Add(item.idd);
                        }
                    }

                }
            }
            return lisDocTableByPallets;
        }

        /* Метод создает единицы работы по документу и виду работы или меняет существующие задания.
            */
        private bool processDocSpecRashoda()
        {
            // 1. поиск всех существующих не помеченных на удаление заданий по документу и виду работы
            XPQuery<MobileWorkUnit> queryRealWorkUnits = new XPQuery<MobileWorkUnit>(objectSpace.Session());

            var listRealUnits = from o in queryRealWorkUnits
                                where
                                o.BaseDoc.idd == currentDoc.idd &&
                                o.JobType.idd == currentJobe.idd &&
                                o.IsMarkDeleted != true
                                select o;

            // 2. разделение табличной части документа на паллеты:
            List<DocTableByPallets> docTableByPallets = getPalletsFromDocTableSpecRashoda(currentDoc);
            if ((docTableByPallets == null) || (docTableByPallets.Count == 0))
            {
                return false;
            }

            // 3. привязка к табличной части существующих заданий
            linkMobileWorkUnitTodocTablePallets(docTableByPallets, listRealUnits);

            // 4. попытка изменения статуса существующего задания, только если оно не выполнено или не в работе.
            foreach (var item in docTableByPallets)
            {
                if (item.mobileWorkUnit != null)
                {
                    if (newType == enTypesOfMobileStates.ВПроцессеОтзыва)
                    {
                        item.mobileWorkUnit.TypeOfMobileState = enTypesOfMobileStates.ОтозванИзРаботы;
                    }
                    else if (newType == enTypesOfMobileStates.ВПроцессеОтправки)
                    {
                        item.mobileWorkUnit.TypeOfMobileState = enTypesOfMobileStates.ДоступенДляРаботы;
                    }
                    else if (item.mobileWorkUnit.TypeOfMobileState != newType)
                    {
                        item.mobileWorkUnit.TypeOfMobileState = newType;
                    }
                    item.mobileWorkUnit.Save();
                }
            }

            // 5. создание новых заданий, которых еще нет в БД:            
            Users current_User = (Users)SecuritySystem.CurrentUser;
            Users currentUser = objectSpace.FindObject<Users>(new BinaryOperator("idd", current_User.idd, BinaryOperatorType.Equal));
            Delimeters delimeter = objectSpace.FindObject<Delimeters>(new BinaryOperator("DelimeterID", current_User.DefaultDelimeter.DelimeterID, BinaryOperatorType.Equal), true);

            XPQuery<DocSpecRashodaGoods> queryDocTable = new XPQuery<DocSpecRashodaGoods>(objectSpace.Session());

            foreach (var item in docTableByPallets)
            {
                if (item.mobileWorkUnit == null)
                {
                    //Nullable<Decimal> quantity_Plan = (from o in item.listOfDocTable select o.QuantityOrig).Sum();
                    //var quantity_Plan = (from o in queryDocTable where item.listOfDocTable.Contains(o.idd) select o.QuantityOrig).Sum();
                    var quantity_Plan = queryDocTable.Where(o => item.listOfDocTable.Contains(o.idd)).Sum(o => o.QuantityOrig);

                    //Decimal quantityPlan = quantity_Plan ?? (decimal)Decimal.Zero;

                    MobileWorkUnit newMWUnit = new MobileWorkUnit(objectSpace.Session())
                    {
                        BaseDoc = currentDoc,
                        JobType = item.currentJobeType,
                        AssignedUser = null,
                        TypeOfMobileState = enTypesOfMobileStates.ДоступенДляРаботы,
                        SendedUser = currentUser,
                        Delimeter = delimeter,
                        dTimeInsertUnit = DateTime.Now,
                        TypeOfUnitWork = enTypesOfUnitWork.НеОпределен,
                        Barcode = "",
                        QuantityPlan = quantity_Plan,
                        QuantityFact = 0
                    };
                    newMWUnit.Save();

                    // добавление строк документа, по которым будет выполняться размещение:
                    foreach (var itemRow in item.listOfDocTable)
                    {
                        DocSpecRashodaGoods itemDoc = objectSpace.Session().GetObjectByKey<DocSpecRashodaGoods>(itemRow, true);
                        if (itemDoc == null)
                        {
                            logger.Warn("processDocSpecRashoda: ошибка поиска строки документа по idd. документ: {0},iddoc={1} idd строки={2}", currentDoc.ToString(), currentDoc.idd, itemRow);
                            continue;
                        }
                        MobileWorkDocRows newDocRow = new MobileWorkDocRows(objectSpace.Session())
                        {
                            BaseDocRowId = itemDoc.idd,
                            BaseDocRowGUID = itemDoc.DocLineUID,
                            idGUID = Guid.NewGuid(),
                            Delimeter = delimeter,
                            LineNo = itemDoc.LineNo
                        };
                        newDocRow.Save();
                        newMWUnit.MobileWorkDocRowsCollection.Add(newDocRow);
                    }
                }
            }

            // 6. удаление существующих не выполненных заданий, которые не удалось привязать к табличной части документа
            var iddRealUnits = from o in docTableByPallets where o.mobileWorkUnit != null select o.mobileWorkUnit.idd;

            var deletedUnits = from o in listRealUnits
                               where
                                   //o.TypeOfMobileState==enTypesOfMobileStates.ДоступенДляРаботы
                                !iddRealUnits.Contains(o.idd)
                               select o;
            foreach (var item in deletedUnits)
            {
                item.Delete();
            }

            objectSpace.CommitChanges();
            return true;
        }
      
        #endregion
  
        
        /* Класс служит для группировки табличной части документа по паллетам с учетом товара, характеристики, срока годности
        
         */
        private class DocTableByPallets
        {
            public List<int> listOfDocTable {get; set;}// список идентификаторов строк документа
            public String strPalletCodeTakeFrom;// код паллета-отправителя
            public String strPalletCodePutTo;// код паллета-получателя
            public MobileWorkUnit mobileWorkUnit;// существующая или созданная единица работы по строкам документа
            public enTypeOfWorks typeOfWork;// вид работы по текущей паллете: ПеремещениеИзТочкиПередачи, Перемещение, ПеремещениеВТочкуПередачи и т.д.
            public JobTypes currentJobeType;// элемент справочника видов работ, присваивается для каждой паллеты  индивидуально

            public DocTableByPallets()
            {
                listOfDocTable = new List<int>();
            }
        }
    }
}
