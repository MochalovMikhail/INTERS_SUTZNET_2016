using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SUTZ_2.Module.BO.References;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Utils;
using System.Diagnostics;
using System.Windows.Forms;
using SUTZ_2.Module;
using NLog;
using DevExpress.Persistent.Base;
using SUTZ_2.Module.BO;
using DevExpress.Data.Browsing;
using System.Collections;
using DevExpress.Xpo.DB.Exceptions;
using SUTZ_2.Module.BO.Documents;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using SUTZ_2.Module.BO.References.Mobile;
using DevExpress.ExpressApp.Xpo;
using System.Drawing;

namespace SUTZ_2.MobileSUTZ
{
  
    [NonPersistent]
    internal class FreePallet
    {
        internal int docIDD { get; set; }// идентификатор документа спец.прихода
        internal String strPalletCode { get; set; }// код паллета
        internal String strSSCCCode { get; set; }// код паллетной этикетки
        internal Decimal totalQuantity { get; set; } // количество товара в паллете
        internal List<int> listActualDocRowIDs { get; set; }// сохраняет идентификаторы строк спец.прихода между запросом сканирования ШК этикетки И методом сохраненения изменения в БД 
    }
    // перечисление, необходимое для определения этапа сканирования:
   // [NonPersistent]
    internal enum enumScanState
    {
        НачалоРаботы = 1,
        ОтсканированДокумент = 2,
        ОтсканированТовар = 3,
        ОтсканированСрокГодности = 4,
        ОтсканированНомерПартии = 5,
        ОтсканированСерийныйНомер = 6,
        ОтсканированоКоличество = 7        
    }

    [NonPersistent]
    //[VisibleInDetailView(false), VisibleInListView(false)]
    class prihodPalletLabeling : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();        
        public IObjectSpace objSpace {get; set; }
        public BaseDocument currentDocBase { get; set;}

        // локальные переменные, необходимые для работы бизнес-логики:
        private localClassScanState scanStateFields { get; set; }

        #region Члены IDisposable

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        
        protected virtual void Dispose(bool disposing)
        {
        }

        public prihodPalletLabeling(IObjectSpace paramObjSpace)
        {
            Guard.ArgumentNotNull(paramObjSpace, "paramObjSpace");
            objSpace = paramObjSpace;
            scanStateFields = new localClassScanState();
        }
        ~prihodPalletLabeling()
        {
            Dispose(false);
        }
             
        // 2.1 формирует заголовок формы
        private String getCurrentOperationStatusString()
        {
            StringBuilder sb = new StringBuilder();
            // 1. Первый этап: отсканировано Требование на разгрузку
            if (scanStateFields.currentScanState>=enumScanState.ОтсканированДокумент)
            {
                sb.AppendFormat("" + scanStateFields.docTrebovanie);
            }

            // 2. Второй этап: отсканирован товар:
            if (scanStateFields.currentScanState>=enumScanState.ОтсканированТовар)
            {
                sb.AppendFormat("\nТовар: " + scanStateFields.goodFinded);
            }

            // 3. Третий этап: отсканирован срок годности:
            if (scanStateFields.currentScanState >= enumScanState.ОтсканированСрокГодности)
            {
                sb.AppendFormat("\nСрок годности: {0}",scanStateFields.dtTovarDate.ToString("dd.MM.yyyy"));
            }

            // 4. Четвертый этап: отсканирован серийный номер:
            if (scanStateFields.currentScanState >= enumScanState.ОтсканированСерийныйНомер)
            {
                sb.AppendFormat("\nНомер партии: " + scanStateFields.strTovarSN);
            }

            // 5. Пятый этап: введено количество товара:
            if (scanStateFields.currentScanState >= enumScanState.ОтсканированоКоличество)
            {
                sb.AppendFormat("\nКоличество: " + scanStateFields.totalQuantity);
            }

            return sb.ToString();
        }

        // метод определяет список паллетных этикеток, доступных для маркировки; вызывает только после ввода количества товара в паллете,
        private List<FreePallet> getAvailableFreePallets()
        {
            const String strFuncName = "getAvailableFreePallets";
            List<FreePallet> returnValue = new List<FreePallet>();

            // 1.3 выполнение запроса по классу DocJobeProperties:
            IQueryable<int> listOfDocIds = getAvalaibleDocIDs(scanStateFields.docTrebovanie);
            if ((listOfDocIds == null) || (listOfDocIds.Count() == 0))
            {
                logger.Trace("{0} Нет доступных Спец.прихода для маркировки. Требование = {1}", strFuncName, scanStateFields.docTrebovanie);
                return returnValue;
            }

            // 1.4 поиск уже промаркированных строк спец.прихода:
            IQueryable<int> listOfComletedDTId = getCompletedDocTableIDs(listOfDocIds);

            // 1.3 поиск количества паллет в табличной части подчиненных спецификация прихода:
            XPQuery<DocSpecPrihodaGoods> goodsDocSpec = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
            // нужно найти паллет в спец.прихода, который соответствует по товару, сроку годности, количеству (сумма паллета), характеристике и т.д. И Запомнить номер спецификации, код паллета в общей структуре
            if (scanStateFields.currentScanState == enumScanState.ОтсканированСерийныйНомер)
            {

            }

            try
            {
                var grp1 = (from s in goodsDocSpec
                                   where
                                       s.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd
                                       && s.ParentID.DocState == DocStates.Проведен
                                       && s.Good.idd == scanStateFields.goodFinded.idd
                                       && s.BestBefore.Year == scanStateFields.dtTovarDate.Year
                                       && s.BestBefore.Month == scanStateFields.dtTovarDate.Month
                                       && s.BestBefore.Day == scanStateFields.dtTovarDate.Day
                                       && s.Property.Description == scanStateFields.strTovarSN
                                       && s.PalletLabel != null
                                       && s != null
                                       && listOfDocIds.Contains(s.ParentID.idd)
                                       && !listOfComletedDTId.Contains(s.idd)
                                   group s by new { PalletCode = s.PutTo.PalletCode, docID = s.ParentID.idd, SSCC = s.PalletLabel.Barcode } into gpr
                                   select new
                                   {
                                       keyRecord = gpr.Key,
                                       Items = gpr
                                       //TotalQuantity = gpr.Sum(p => p.TotalQuantity),
                                       //CellsCount = gpr.Count()

                                   })
                    //.Where(p => p.TotalQuantity == scanStateFields.totalQuantity)
                                     .OrderBy(a => a.keyRecord.docID)
                                     .ThenBy(b => b.keyRecord.PalletCode);

                if (grp1 == null)
                {
                    //paramScan.captionOne = String.Format("Не найдена паллета с количеством = {0} штук", scanStateFields.totalQuantity);
                    logger.Warn("{0} В табличной части не найдена паллета с количеством ={0}, Требование на разгрузку={1}", strFuncName, scanStateFields.totalQuantity, scanStateFields.docTrebovanie);
                    return returnValue;
                }
                foreach (var item in grp1)
                {
                    decimal palletSum = item.Items.Sum(p => p.TotalQuantity);
                    //foreach (var item2 in item.Items)
                    //{
                    //    decimal quantity = item2.QuantityOfItems;
                    //    Debug.WriteLine("Storage code="+item2.PutTo.Code+", quantity = " + quantity);
                    //}
                    if (palletSum!=scanStateFields.totalQuantity)
                    {
                        continue;
                    }
                    returnValue.Add(new FreePallet
                    {
                        docIDD = item.keyRecord.docID,
                        strPalletCode = item.keyRecord.PalletCode,
                        strSSCCCode = item.keyRecord.SSCC,
                        totalQuantity = palletSum,
                        listActualDocRowIDs = item.Items.Select(s=>s.idd).ToList<int>()
                    });
                }

                //остановился здесь: нужно сохранять все коды паллет, коды подходят подходят под условия выборки.!!!
                // установка в общей структуре найденных параметров: 
                //scanStateFields.strPalletCode = firstPallet.PalletCode;
                //scanStateFields.docSpecProhoda = firstPallet.docSpecPrih;

            }
            catch (System.ArgumentNullException ex)
            {
                //paramScan.captionOne = String.Format("Ошибка при выполнении запроса.\n Попробуйте еще раз!");
                logger.Error("{0} Ошибка выполнения запроса ArgumentNullException: ex.ParamName={1}, ex.Message={2}, ex.StackTrace={3}", strFuncName, ex.ParamName, ex.Message, ex.StackTrace);
                return returnValue;
            }
            catch (System.Exception ex)
            {
                //paramScan.captionOne = String.Format("Ошибка при выполнении запроса. \n Попробуйте еще раз!");
                logger.Error("{0} Ошибка выполнения запроса Exception: ex.InnerException={1}, ex.Message={2}, ex.StackTrace={3}", strFuncName, ex.InnerException.ToString(), ex.Message, ex.StackTrace);
                return returnValue;
            }

            return returnValue;
        }

        // метод выполняет поиск всех спец.прихода, у которых установлен признак доступности документа для работы:
        private IQueryable<int> getAvalaibleDocIDs(BaseDocument baseDoc)
        {
            XPQuery<DocJobesProperties> xpQuery = new XPQuery<DocJobesProperties>(objSpace.Session());
            var listOfIdd = from c in xpQuery
                                        where
                                        c.BaseDoc.DocBase.idd == baseDoc.idd &&
                                        c.TypeOfMobileState == enTypesOfMobileStates.ДоступенДляРаботы &&
                                        c.JobType.idd == scanStateFields.sprJobeType.idd
                                        select c.BaseDoc.idd;
            int countElement = listOfIdd.Count();// выполнение запроса.
            return listOfIdd;
        }

        // метод выполняет поиск всех строк спец.прихода, которые были промаркированы:
        // передаваемые параметры: docIDs - список IDD документов, по которым нужно выполнять поиск.
        private IQueryable<int> getCompletedDocTableIDs(IQueryable<int> docIDs)
        {
            XPQuery<MobileWorkDocRows> xpQuery = new XPQuery<MobileWorkDocRows>(objSpace.Session());
            var listOfCompletedIdd = from c in xpQuery
                                                 where
                                                 docIDs.Contains(c.ParentId.BaseDoc.idd)
                                                 && c.ParentId.JobType.idd == scanStateFields.sprJobeType.idd
                                                 select c.BaseDocRowId;
            int countElement = listOfCompletedIdd.Count();// выполнение запроса.
            return listOfCompletedIdd;
        }
        
        // метод выполняет поиск всех строк спец.прихода, которые НЕ были промаркированы:
        // передаваемые параметры: docIDs - список IDD документов, по которым нужно выполнять поиск.
        private IQueryable<int> getNotCompletedDocTableIDs(BaseDocument baseDoc)
        {
            // 1. получение доступных документов: ссылка на требование уже установлена в общей структуре.
            IQueryable<int> availableDocIds = getAvalaibleDocIDs(baseDoc);

            // 2. получение выполенных строк документа:            
            IQueryable<int> completedDocRowIds = getCompletedDocTableIDs(availableDocIds);
            
            // 3. выполнение запроса по всем строкам документов с фильтром по выполненным строкам документов:

            //List<int> var1 = new List<int> { 1, 2, 3 };

            XPQuery<DocSpecPrihodaGoods> xpQuery = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
            var listOfNotCompletedIds = (from c in xpQuery
                                                     where
                                                     availableDocIds.Contains(c.ParentID.idd)                                                     
                                                     select c.idd)
                                                     .Except(completedDocRowIds).AsQueryable();
                                                 
            return listOfNotCompletedIds;
        }
        
        // 1. стартовая процедура, вызываемая при начале работы модуля
        public bool runScanBeginPriemMarkingGoods(JobTypes selectedJobType)
        {
            logger.Trace("Вход в функцию, selectedJobtype = {0}",selectedJobType);
            scanStateFields.sprJobeType = selectedJobType;

            // 1. вызов диалога сканирования ШК документа Требование на разгрузку:
            BaseDocument docFinded = scanDocTrebovanie();
            if (docFinded == null)
            {
                logger.Trace("Выход из функции runScanBeginPriemMarkingGoods(), т.к. ошибка при сканировании документа.");
                return false;
            }
            if (selectedJobType == null)
            {
                logger.Trace("Выход из функции runScanBeginPriemMarkingGoods(), т.к. параметр selectedJobType = null.");
                return false;
            }

            // 2. установка начальных настроек 
            scanStateFields.docTrebovanie = docFinded;
            scanStateFields.currentScanState = enumScanState.ОтсканированДокумент;            

            logger.Trace("в справочнике currentSessionSettings.CurrentDocBase установлен документ = {0}", docFinded.ToString());

            // 3. вызов основной процедуры присвоение СН товарам
            return runMainLoop(docFinded);
        }

        // 2. метод для вызова диалога сканирования ШК пропуска на въезд.
        private BaseDocument scanDocTrebovanie()
        {
            structScanStringParams paramScan = new structScanStringParams() {
                captionOne = "",
                captionTwo = "Отсканируйте пропуск на въезд:", 
                successScan = false,
                enableQuestionButton = false
            };

            // 2. вызов диалога сканирования ШК
            UserSelect userSelect = new UserSelect();
            BaseDocument docFinded = null;
            while (true)
            {
                docFinded = null;
                logger.Trace("Перед вызовом сканирования ШК Требования на разгрузку = {0}");
                DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("Диалог сканирования ШК вернул = {0}, выход из функции", dlgResult);
                    return null;
                }
                if (!paramScan.successScan)
                {
                    logger.Trace("paramScan.successScan = {0}, выход из функции.", paramScan.successScan);
                    return null;
                }
                string strBarcode = paramScan.scanedBarcode;
                logger.Trace("отсканирован ШК = {0}", strBarcode);

                // 3. поиск документа требование на разгрузку по ШК:
                Guid guidParsed = new Guid();

                if (!Guid.TryParse(strBarcode, out guidParsed))
                {
                    paramScan.captionOne = "Ошибка формата штрих-кода документа Требование на разгрузку!";
                    logger.Trace("Ошибка поиска документа Требование на разгрузку по ШК: ошибка при попытке создать GUID из строки = {0}", strBarcode);
                    continue;
                }
                    
                docFinded = objSpace.FindObject<BaseDocument>(new BinaryOperator("idGUID", guidParsed, BinaryOperatorType.Equal), true);                
                if (docFinded == null)
                {
                    paramScan.captionOne = "Ошибка поиска Требования на разгрузку по штрихкоду!";
                    logger.Trace("ошибка поиска документа Требование на разгрузку по ШК = {0}", strBarcode);
                    continue;
                }
                // 4. определение вида документа
                if (docFinded.ClassInfo.ClassType != typeof(DocTrebovanie))
                {
                    paramScan.captionOne = "Ошибка определения вида найденного документа! ";
                    logger.Trace("Ошибка определения вида найденного документа. Ожидался Требование на разгрузку, был отсканирован = {0}, вид документа ={1}", docFinded.DocNo, docFinded.ClassInfo.ClassType.ToString());
                    continue;
                }

                // 5. проверка документа, что в нем остались не выполненные строки:
                IQueryable<int> notCompletedDocRows = getNotCompletedDocTableIDs(docFinded);
                if ((notCompletedDocRows == null) || (notCompletedDocRows.Count() == 0))
                {
                    string strMessage = String.Format("В документе {0} нет доступных для маркировки паллет!", docFinded.DocNo);
                    paramScan.captionOne = strMessage;
                    logger.Trace("{0}, doc id= {1}", strMessage,docFinded.idd);
                    continue;
                }
                break;
            }
            return docFinded;
        }

        // 3.1 вызов диалога сканирования и проверки товара
        private DialogResult lokScanTovar() 
        {
            const string strFuncName = "lokScanTovar:";

            // 1. вызов диалога сканирования товара
            structScanStringParams paramScan = new structScanStringParams() 
            { 
                captionTwo = "Сканируйте товар:", 
                captionHelp = getCurrentOperationStatusString(), 
                successScan = false,
                enableQuestionButton = true
            };

            UserSelect userSelect = new UserSelect();
            Goods goodFinded1 = null;

            scanStateFields.dTimeStartOperation = DateTime.Now;

            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом сканирования ШК товара", strFuncName);
                // 1. вызов диалога сканирования товара
                DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог сканирования ШК товара вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }
                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }
                
                string strBarcode = paramScan.scanedBarcode;
                logger.Trace("{0} отсканирован ШК товара = {1}", strBarcode);

                // 1.2 поиск товара по ШК:
                goodFinded1 = new RefGlobalMetods() { ObjectSpace = objSpace}.findGoodByEANCode(strBarcode);
                if (goodFinded1 == null)
                {
                    paramScan.captionOne = "Не найден товар по штрихкоду: '" + strBarcode+"'";
                    logger.Trace("{0} ошибка поиска товара в справочнике по ШК:'{1}'", strFuncName, strBarcode);
                    continue;
                }
              
                // 1.3 выполнение запроса по классу DocJobeProperties:
                IQueryable<int> listOfDocIds = getAvalaibleDocIDs(scanStateFields.docTrebovanie);
                if ((listOfDocIds==null)||(listOfDocIds.Count()==0))
                {
                    paramScan.captionOne = "Нет доступных Спец.прихода для маркировки!";
                    logger.Trace("{0} Нет доступных Спец.прихода для маркировки. Требование = {1}", strFuncName, scanStateFields.docTrebovanie);
                    continue;
                }
                // 1.4 поиск уже промаркированных строк спец.прихода:
                IQueryable<int> listOfComletedDTId = getCompletedDocTableIDs(listOfDocIds);

                // 1.5 поиск товара в табличной части подчиненных спецификациях прихода:
                XPQuery<DocSpecPrihodaGoods> goodsDocSpec = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session()); 
                var listOfGoods = from c in goodsDocSpec where
                                      listOfDocIds.Contains(c.ParentID.idd)
                                      && !listOfComletedDTId.Contains(c.idd)
                                      && c.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd 
                                      && c.ParentID.DocState==DocStates.Проведен 
                                      && c.Good.idd == goodFinded1.idd
                                      && c.PalletLabel != null
                                  select c.idd;

                if (listOfGoods.Count() == 0)
                {
                    paramScan.captionOne = String.Format("В приходе нет доступного товара\n {0}",goodFinded1.ShortDescription);
                    logger.Trace("{0} ошибка поиска товара {1} в табличной части подч.Спец.Прихода по требованию {2}", strFuncName, goodFinded1, scanStateFields.docTrebovanie);
                    continue;
                }
                // установка в общем классе отсканированного значения
                scanStateFields.goodFinded = goodFinded1;
                //scanStateFields.listActualDocRowIDs = listOfGoods.ToList<int>();
                break;
            }
            return DialogResult.OK;
        }

          // 3.2 вызов диалога ввода срока годности:      
        private DialogResult lokInputTovarDate()
        {
            const string strFuncName = "lokInputTovarDate:";

            // 1. вызов диалога сканирования срока годности товара:
            structScanStringParams paramScan = new structScanStringParams() 
            { 
                captionOne = "",
                captionTwo = "Введите срок годности товара:", 
                captionHelp = getCurrentOperationStatusString(),
                enableQuestionButton = true
            };

            UserSelect userSelect = new UserSelect();

            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом ввода даты товара", strFuncName);
                // 1. вызов диалога ввода даты товара
                DialogResult dlgResult = userSelect.scanDateDialog(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог ввода даты товара вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }
                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }
                
                string strBarcode = paramScan.scanedBarcode;
                logger.Trace("{0} Введена дата товара = {1}", strFuncName, strBarcode);

                bool dateTimeTryParse = false;
                DateTime dtTovarDate = new DateTime(2001,1,1);
                try
                {
                    dateTimeTryParse = DateTime.TryParse(strBarcode, out dtTovarDate);
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex, strFuncName+" Исключение при преобразовании строки '"+strBarcode+"' в дату");
                }

                if (!dateTimeTryParse)
                {
                    paramScan.captionOne = "Ошибка определения даты: " + strBarcode;
                    logger.Trace("{0} ошибка определения даты из строки = {1}",strFuncName, strBarcode);
                    continue;                
                }

                // 1.3 выполнение запроса по классу DocJobeProperties:
                IQueryable<int> listOfDocIds = getAvalaibleDocIDs(scanStateFields.docTrebovanie);
                if ((listOfDocIds == null) || (listOfDocIds.Count() == 0))
                {
                    paramScan.captionOne = "Нет доступных Спец.прихода для маркировки!";
                    logger.Trace("{0} Нет доступных Спец.прихода для маркировки. Требование = {1}", strFuncName, scanStateFields.docTrebovanie);
                    continue;
                }
                // 1.4 поиск уже промаркированных строк спец.прихода:
                IQueryable<int> listOfComletedDTId = getCompletedDocTableIDs(listOfDocIds);

                // 1.5 поиск товара и срока годности в табличной части подчиненных спецификация прихода:
                XPQuery<DocSpecPrihodaGoods> goodsDocSpec = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
                DateTime newDateTime = new DateTime(dtTovarDate.Ticks);

                var listOfGoods = from c in goodsDocSpec
                                  where
                                  listOfDocIds.Contains(c.ParentID.idd)
                                      && !listOfComletedDTId.Contains(c.idd)
                                      && c.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd
                                      && c.ParentID.DocState == DocStates.Проведен
                                      && c.Good.idd == scanStateFields.goodFinded.idd
                                      && c.BestBefore.Year == newDateTime.Year
                                      && c.BestBefore.Month == newDateTime.Month
                                      && c.BestBefore.Day == newDateTime.Day
                                      && c.PalletLabel != null
                                  select c.idd;
               
                if (listOfGoods.Count() == 0)
                {
                    paramScan.captionOne = String.Format("В приходе по {0} не найден срок годности:\n{1}",scanStateFields.docTrebovanie, newDateTime.ToString("dd.MM.yyyy"));
                    logger.Trace("{0} ошибка поиска товара: {1} по сроку годности:{4} в табличной части подч.Спец.Прихода по требованию:{3}", strFuncName, scanStateFields.goodFinded, scanStateFields.docTrebovanie, newDateTime);
                    continue;
                }
                scanStateFields.dtTovarDate = dtTovarDate;
                //scanStateFields.listActualDocRowIDs = listOfGoods.ToList<int>();
                break;
            }
            return DialogResult.OK;
        }

        // 3.3 вызов диалога ввода серийного номера товара:
        private DialogResult lokScanSNTovar()
        {
            const string strFuncName = "lokScanSNTovar:";

            // 1. вызов диалога сканирования товара
            structScanStringParams paramScan = new structScanStringParams() 
            { 
                captionTwo = "Сканируйте номер партии товара:",
                successScan = false,
                captionHelp = getCurrentOperationStatusString(),
                enableQuestionButton = true
            };            
            
            UserSelect userSelect = new UserSelect();

            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом сканирования СН товара", strFuncName);
                // 1. вызов диалога сканирования серийного номера:
                DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог сканирования СН товара вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }
                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }

                string strBarcode = paramScan.scanedBarcode;
                logger.Trace("{0} отсканирован СН товара ={1}", strFuncName, strBarcode);

                // 1.3 выполнение запроса по классу DocJobeProperties:
                IQueryable<int> listOfDocIds = getAvalaibleDocIDs(scanStateFields.docTrebovanie);
                if ((listOfDocIds == null) || (listOfDocIds.Count() == 0))
                {
                    paramScan.captionOne = "Нет доступных Спец.прихода для маркировки!";
                    logger.Trace("{0} Нет доступных Спец.прихода для маркировки. Требование = {1}", strFuncName, scanStateFields.docTrebovanie);
                    continue;
                }
                // 1.4 поиск уже промаркированных строк спец.прихода:
                IQueryable<int> listOfComletedDTId = getCompletedDocTableIDs(listOfDocIds);

                // 1.5 поиск товара в табличной части подчиненных спецификация прихода:
                XPQuery<DocSpecPrihodaGoods> goodsDocSpec = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
                var listOfGoods = from c in goodsDocSpec
                                  where
                                      c.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd
                                      && c.ParentID.DocState == DocStates.Проведен
                                      && c.Good.idd == scanStateFields.goodFinded.idd
                                      && c.BestBefore.Year == scanStateFields.dtTovarDate.Year
                                      && c.BestBefore.Month == scanStateFields.dtTovarDate.Month
                                      && c.BestBefore.Day == scanStateFields.dtTovarDate.Day
                                      && c.Property.Description == strBarcode
                                      && c.PalletLabel!=null
                                      && listOfDocIds.Contains(c.ParentID.idd)
                                      && !listOfComletedDTId.Contains(c.idd)
                                  select c.idd;

                if (listOfGoods.Count() == 0)
                {
                    paramScan.captionOne = String.Format("В приходе по {0} не найден номер партии:\n{1}",scanStateFields.docTrebovanie, strBarcode);
                    logger.Trace("{0} Ошибка поиска товара {1} в табличной части подч.Спец.Прихода по требованию {2}",strFuncName, scanStateFields.goodFinded, scanStateFields.docTrebovanie);
                    continue;
                }
                scanStateFields.strTovarSN = strBarcode;
                //scanStateFields.listActualDocRowIDs = listOfGoods.ToList<int>();
                break;
            }
            return DialogResult.OK;
        }

        // 3.4 вызов диалога ввода количества товара:
        private DialogResult lokInputQuantityOfPallet()
        {
            const string strFuncName = "lokInputQuantityOfPallet:";

            // 1. вызов диалога сканирования товара
            structScanStringParams paramScan = new structScanStringParams() 
            {   captionTwo = "Введите количество товара:", 
                successScan = false, 
                captionHelp = getCurrentOperationStatusString(), 
                unitFactCalculate = null,
                enableQuestionButton = true
            };

            // 1. определение единицы измерения, по которой будет выполняться расчет:
            XPQuery<DocSpecPrihodaGoods> queryOfDoc = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
            var rowListOfDoc = (from c in queryOfDoc
                           where
                               c.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd
                               && c.ParentID.DocState == DocStates.Проведен
                               && c.Good.idd == scanStateFields.goodFinded.idd
                               && c.BestBefore.Year == scanStateFields.dtTovarDate.Year
                               && c.BestBefore.Month == scanStateFields.dtTovarDate.Month
                               && c.BestBefore.Day == scanStateFields.dtTovarDate.Day
                               && c.Property.Description == scanStateFields.strTovarSN
                           select c).FirstOrDefault();
            if (rowListOfDoc!=null)
            {
                paramScan.unitFactCalculate = rowListOfDoc.Unit;
            }  
            UserSelect userSelect = new UserSelect(); 
            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом ввода количества товара.", strFuncName);

                // 1. вызов диалога ввода количества товара:
                DialogResult dlgResult = userSelect.inputQantityOfUnits(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог ввода количества товара вернул = {1}", strFuncName, dlgResult);
                    return dlgResult;
                }
                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }

                logger.Trace("{0} Диалог ввода количества товара вернул = {0}", strFuncName, paramScan.decFactTotal);
                scanStateFields.totalQuantity = paramScan.decFactTotal;

                // 1.3 выполнение запроса по классу DocJobeProperties:
                IQueryable<int> listOfDocIds = getAvalaibleDocIDs(scanStateFields.docTrebovanie);
                if ((listOfDocIds == null) || (listOfDocIds.Count() == 0))
                {
                    paramScan.captionOne = "Нет доступных Спец.прихода для маркировки!";
                    logger.Trace("{0} Нет доступных Спец.прихода для маркировки. Требование = {1}",strFuncName, scanStateFields.docTrebovanie);
                    continue;
                }

                // 1.4 новая версия модуля: вызов единого метода для определения списка доступных паллет:
                List<FreePallet> listOfPallets = getAvailableFreePallets();
                if ((listOfPallets == null)||(listOfPallets.Count==0))
                {
                    paramScan.captionOne = String.Format("Не найдена паллета с количеством = {0} штук", scanStateFields.totalQuantity);
                    logger.Warn("{0} В табличной части не найдена паллета с количеством ={1}, Требование на разгрузку={2}", strFuncName, scanStateFields.totalQuantity, scanStateFields.docTrebovanie);
                    continue;
                }
                scanStateFields.listActualFreePallets = listOfPallets;
                
                scanStateFields.strPalletCode = listOfPallets.First().strPalletCode;
                scanStateFields.idddocSpecProhoda = listOfPallets.First().docIDD;

                // 1.4 поиск уже промаркированных строк спец.прихода:
                //IQueryable<int> listOfComletedDTId = getCompletedDocTableIDs(listOfDocIds);

                //// 1.3 поиск количества паллет в табличной части подчиненных спецификация прихода:
                //XPQuery<DocSpecPrihodaGoods> goodsDocSpec = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
                //// нужно найти паллет в спец.прихода, который соответствует по товару, сроку годности, количеству (сумма паллета), характеристике и т.д. И Запомнить номер спецификации, код паллета в общей структуре
                //try
                //{
                //   var firstPallet = (from s in goodsDocSpec
                //                    where
                //                        s.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd
                //                        && s.ParentID.DocState == DocStates.Проведен
                //                        && s.Good.idd == scanStateFields.goodFinded.idd
                //                        && s.BestBefore.Year == scanStateFields.dtTovarDate.Year
                //                        && s.BestBefore.Month == scanStateFields.dtTovarDate.Month
                //                        && s.BestBefore.Day == scanStateFields.dtTovarDate.Day
                //                        && s.Property.Description == scanStateFields.strTovarSN
                //                        && s.PalletLabel!=null
                //                        && s != null
                //                        && listOfDocIds.Contains(s.ParentID.idd)
                //                        && !listOfComletedDTId.Contains(s.idd)
                //                     group s by new {PalletCode=s.PutTo.PalletCode, docID=s.ParentID } into gpr
                //                        select new
                //                        {
                //                            PalletCode = gpr.Key.PalletCode,
                //                            docSpecPrih = gpr.Key.docID,
                //                            TotalQuantity=gpr.Sum(p => p.TotalQuantity),
                //                            CellsCount = gpr.Count()
                //                        })
                //                        .Where(p => p.TotalQuantity == scanStateFields.totalQuantity)
                //                        .OrderBy(a => a.docSpecPrih)
                //                        .ThenBy(b => b.PalletCode)
                //                        .FirstOrDefault();

                //   if (firstPallet == null)
                //   {
                //       paramScan.captionOne = String.Format("Не найдена паллета с количеством = {0} штук", scanStateFields.totalQuantity);
                //       logger.Warn("{0} В табличной части не найдена паллета с количеством ={0}, Требование на разгрузку={1}", strFuncName, scanStateFields.totalQuantity, scanStateFields.docTrebovanie);
                //       continue;
                //   }

                //    //остановился здесь: нужно сохранять все коды паллет, коды подходят подходят под условия выборки.!!!
                //    // установка в общей структуре найденных параметров: 
                //   scanStateFields.strPalletCode = firstPallet.PalletCode;
                //   scanStateFields.docSpecProhoda = firstPallet.docSpecPrih;

                //}
                //catch (System.ArgumentNullException ex)
                //{
                //    paramScan.captionOne = String.Format("Ошибка при выполнении запроса.\n Попробуйте еще раз!");                    
                //    logger.Error("{0} Ошибка выполнения запроса ArgumentNullException: ex.ParamName={1}, ex.Message={2}, ex.StackTrace={3}",strFuncName, ex.ParamName, ex.Message, ex.StackTrace);
                //    continue;
                //}
                //catch (System.Exception ex)
                //{
                //    paramScan.captionOne = String.Format("Ошибка при выполнении запроса. \n Попробуйте еще раз!");
                //    logger.Error("{0} Ошибка выполнения запроса Exception: ex.InnerException={1}, ex.Message={2}, ex.StackTrace={3}",strFuncName, ex.InnerException.ToString(), ex.Message, ex.StackTrace);
                //    continue;
                //}
                break;
            }
            return DialogResult.OK;
        }

        // 3.5 вызов диалога ввода серийного номера паллетной этикетки,
        // закрепление паллетной этикетки за строкой спецификации прихода.
        private DialogResult lokInputPalletLabelSerialNumber()
        {
            const String strFuncName = "lokInputPalletLabelSerialNumber";

            // 1. вызов диалога сканирования паллетной этикетки:
            structScanStringParams paramScan = new structScanStringParams() 
            {         
                captionTwo = "Сканируйте паллетную этикетку:", 
                captionOne = String.Format("Код паллета: {0}", scanStateFields.strPalletCode), 
                scanedBarcode = "", 
                successScan = false, 
                captionHelp = getCurrentOperationStatusString(),
                enableQuestionButton = true
            };

            UserSelect userSelect = new UserSelect();

            while (1 == 1)
            {
                logger.Trace("{0} Перед вызовом сканирования СН паллетной этикетки", strFuncName);

                // 1.3 выполнение запроса по классу DocJobeProperties:
                IQueryable<int> listOfDocIds = getAvalaibleDocIDs(scanStateFields.docTrebovanie);
                if ((listOfDocIds == null) || (listOfDocIds.Count() == 0))
                {
                    paramScan.captionOne = "Нет доступных Спец.прихода для маркировки!";
                    logger.Trace("{0} Нет доступных Спец.прихода для маркировки. Требование = {0}", strFuncName, scanStateFields.docTrebovanie);
                    continue;
                }
                // новая версия модуля:
                List<FreePallet> listOfPallets = getAvailableFreePallets();

                if ((listOfPallets == null) || (listOfPallets.Count() == 0))
                {
                    paramScan.captionOne = "Не найдено ни одной доступной паллеты для маркировки!";
                    logger.Trace("{0} Не найдено ни одной доступной паллеты для маркировки по документу {0}",strFuncName, scanStateFields.docTrebovanie);
                    return DialogResult.Cancel;
                }

                // 3. вызов диалога сканирования серийного номера:
                DialogResult dlgResult = userSelect.scanStringDialog(ref paramScan);
                if (dlgResult != DialogResult.OK)
                {
                    logger.Trace("{0} Диалог сканирования СН паллетной этикетки вернул = {0}", strFuncName, dlgResult);
                    return dlgResult;
                }
                if (!paramScan.successScan)
                {
                    logger.Trace("{0} paramScan.successScan = {1}", strFuncName, paramScan.successScan);
                    continue;
                }
                logger.Trace("{0} отсканирован СН паллетной этикетки = {0}", strFuncName, paramScan.scanedBarcode);
                
                string strBarcode = PalletLabels.detectPalletBarcodeFromFullBarcode(paramScan.scanedBarcode);
                logger.Trace("{0} Определенный ШК паллета = {0}", strFuncName, strBarcode);

                // 4. поиск элемента паллетной этикетки по отсканированному ШК:
                XPQuery<PalletLabels> qPalletLabels = new XPQuery<PalletLabels>(objSpace.Session());
                PalletLabels palletLabel = (from c in qPalletLabels where c.Barcode == strBarcode select c).FirstOrDefault();

                if (palletLabel == null)
                {
                    paramScan.captionOne = String.Format("Не найдена этикетка по штрих-коду:n{0}",strBarcode);
                    logger.Trace("{0} ошибка поиска объекта PalletLabels по ШК: {0}, документ:{1}", strFuncName, strBarcode, scanStateFields.docTrebovanie);
                    continue;
                }
                palletLabel.Reload();
                // 4.1 проверка статуса паллетной этикетки - она не должна быть ранее наклеена:
                if (  (palletLabel.PalletLabelStatus==enPalletLabelStatus.ЭтикеткаНаОстатке)
                    ||(palletLabel.PalletLabelStatus==enPalletLabelStatus.ЭтикеткаОтгружена) )
                {
                    paramScan.captionOne = String.Format("Паллетная этикетка {0} уже была наклеена!",strBarcode);
                    logger.Trace("{0} Паллетная этикетка уже была наклеена. ШК: {0}, документ:", strFuncName, strBarcode, scanStateFields.docTrebovanie);
                    continue;
                }

                // 5. сравнение штрих-кода отсканированной этикетки и этикетки документа:
                FreePallet firstPallet = listOfPallets.FirstOrDefault(p => p.strSSCCCode==palletLabel.Barcode);// потом исправить: rowsListOfDoc.FirstOrDefault().PalletLabel;
                if (firstPallet==null)
                {
                    paramScan.captionOne = String.Format("Отсканирована неправильная этикетка - требуется: {0}", palletLabel.Barcode);
                    logger.Trace("{0} Отсканирована неправильная этикетка - требуется: {0}", strFuncName, palletLabel.Barcode);
                    continue;
                }

                // 6. сохранение в общей структуре запроса с номерами строк, для обработки в методе сохранения изменений в БД:
                scanStateFields.listActualFreePallets = listOfPallets;
                scanStateFields.listActualDocRowIDs = firstPallet.listActualDocRowIDs.ToList<int>();
                scanStateFields.palletLabel = palletLabel;
                scanStateFields.idddocSpecProhoda = firstPallet.docIDD;
                scanStateFields.dTimeScanPalletLabel = DateTime.Now;
                break;
            }
            return DialogResult.OK;            
        }

        private void testGetPalletsNewFunc()
        {
            // 1.3 поиск количества паллет в табличной части подчиненных спецификация прихода:
            XPQuery<DocSpecPrihodaGoods> goodsDocSpec = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
                // нужно найти паллет в спец.прихода, который соответствует по товару, сроку годности, количеству (сумма паллета), характеристике и т.д. И Запомнить номер спецификации, код паллета в общей структуре

            var firstPallet = from s in goodsDocSpec
                              where
                                  s.ParentID.DocBase.idd == scanStateFields.docTrebovanie.idd
                                  && s.ParentID.DocState == DocStates.Проведен
                              //&& s.Good.idd == scanStateFields.goodFinded.idd
                              //&& s.BestBefore.Year == scanStateFields.dtTovarDate.Year
                              //&& s.BestBefore.Month == scanStateFields.dtTovarDate.Month
                              //&& s.BestBefore.Day == scanStateFields.dtTovarDate.Day
                              //&& s.Property.Description == scanStateFields.strTovarSN
                              //&& s.PalletLabel!=null
                              //&& s != null
                              //&& listOfDocIds.Contains(s.ParentID.idd)
                              //&& !listOfComletedDTId.Contains(s.idd)
                              group s by new { PalletCode = s.PutTo.PalletCode, docID = s.ParentID.idd } into gpr
                              select new
                              {
                                  PalletCode = gpr.Key,
                                  //docSpecPrih = gpr.Key.docID,
                                  // TotalQuantity = gpr.Sum(p => p.TotalQuantity),
                                  // CellsCount = gpr.Count(),
                                  //records = from c in gpr group c by c.idd into g2 select g2
                                  records = gpr
                              };
                                
                               // .Where(p => p.TotalQuantity == scanStateFields.totalQuantity)
                                //.OrderBy(a => a.docSpecPrih)
                                //.ThenBy(b => b.PalletCode)
                                //.FirstOrDefault();

            if (firstPallet!=null)
            {
                foreach (var item in firstPallet)
                {
                    foreach (var item2 in item.records)
                    {
                        int lokidd = item2.idd;
                    }

                }
            }
        }
        // 3. вызов диалога сканирования в бесконечном цикле:
        private bool runMainLoop(BaseDocument baseDocTrebovanie)
        {            
            // 1. приведение базового документа к Требованию на разгрузку:
            while (true)
            {
                //testGetPalletsNewFunc();
                // 3.1 начало цикла, нужно сканировать товар:
                #region сканированиеТовара
               if (scanStateFields.currentScanState==enumScanState.ОтсканированДокумент)
                {
                    // пользователь нажал отмена, товар не найден в документе или товар найден
                    DialogResult dlgResult = lokScanTovar();//(docTrebovanie, ref goodFinded)
                    if (dlgResult!=DialogResult.OK)
                    {
                        logger.Trace("runMainLoop().Диалог сканирования ШК товара вернул = {0}", dlgResult);
                        scanStateFields.currentScanState = enumScanState.ОтсканированДокумент;
                        scanStateFields.goodFinded = null;// обнулим ссылку на товар на всякий случай.
                        return false;
                    }
                    // установим признак, что товар успешно отсканирован;
                    scanStateFields.currentScanState = enumScanState.ОтсканированТовар;
                }
               #endregion

                // 3.2 запрос ввода срока годности товара:
               #region сканированиеСрокаГодности
               if (scanStateFields.currentScanState == enumScanState.ОтсканированТовар)
               {
                   // пользователь нажал отмена, товар не найден в документе или товар найден
                   DialogResult dlgResult = lokInputTovarDate();// (docTrebovanie, ref goodFinded);
                   if (dlgResult != DialogResult.OK)
                   {
                       logger.Trace("runMainLoop(): Диалог ввода срока годности товара вернул = {0}", dlgResult);
                       scanStateFields.currentScanState = enumScanState.ОтсканированДокумент;
                       scanStateFields.dtTovarDate = new DateTime(2001,1,1);// обнулим ссылку на дату товара на всякий случай.
                       continue;
                   }
                   // установим признак, что срок годности успешно отсканирован;
                   scanStateFields.currentScanState = enumScanState.ОтсканированСрокГодности;
               }
               #endregion
               
                // 3.3 вызов диалога ввода серийного номера товара:
                #region СканированиеСерийногоНомераТовара
               if (scanStateFields.currentScanState == enumScanState.ОтсканированСрокГодности)
                {
                    // пользователь нажал отмена, товар не найден в документе или товар найден
                    DialogResult dlgResult = lokScanSNTovar();//docTrebovanie, goodFinded, dtTovarDate, ref strTovarSN);
                    if (dlgResult != DialogResult.OK)
                    {
                        logger.Trace("runMainLoop(). Диалог сканирования SN товара вернул = {0}", dlgResult);
                        scanStateFields.currentScanState = enumScanState.ОтсканированТовар;
                        scanStateFields.strTovarSN = "";// обнулим ссылку на СН товара на всякий случай.
                        continue;
                    }
                    
                    // установим признак, что серийный номер успешно отсканирован;
                    scanStateFields.currentScanState = enumScanState.ОтсканированСерийныйНомер;
                }
               #endregion
              
               // 3.4 вызов диалога ввода фактического количества товара
               #region ВводКоличестваТовара
               if (scanStateFields.currentScanState == enumScanState.ОтсканированСерийныйНомер)
               {
                   // пользователь нажал отмена, товар не найден в документе или товар найден
                   DialogResult dlgResult = lokInputQuantityOfPallet();//docTrebovanie, goodFinded, dtTovarDate, strTovarSN, totalQuantity);
                   if (dlgResult != DialogResult.OK)
                   {
                       logger.Trace("runMainLoop(). Диалог ввода количества товара вернул = {0}", dlgResult);
                       scanStateFields.currentScanState = enumScanState.ОтсканированСрокГодности;
                       //totalQuantity = 0;
                       scanStateFields.totalQuantity =0;
                       continue;
                   }
                   // установим признак, что серийный номер успешно отсканирован;
                   scanStateFields.currentScanState = enumScanState.ОтсканированоКоличество;
               }
               #endregion

               // 3.5 вызов диалог сканирования СН паллетной этикетки:
               #region СканированиеСНПаллетнойЭтикетки
               if (scanStateFields.currentScanState == enumScanState.ОтсканированоКоличество)
               {
                   // 3.5.1 пользователь нажал отмена, товар не найден в документе или товар найден
                   DialogResult dlgResult = lokInputPalletLabelSerialNumber();
                   if (dlgResult != DialogResult.OK)
                   {
                       logger.Trace("runMainLoop(). Сканирование SN паллетной этикетки = {0}", dlgResult);
                       scanStateFields.currentScanState = enumScanState.ОтсканированСерийныйНомер;
                       //totalQuantity = 0;
                       scanStateFields.totalQuantity = 0;
                       continue;
                   }
                   // 3.5.2 выполнение сохранения результатов маркировки в БД:
                   if (lokSaveAllDateToDataBase())
                   {
                       // 3.5.3установим признак, что серийный номер успешно отсканирован;
                       structScanStringParams newParams = new structScanStringParams()
                       {
                           captionOne = "",
                           captionTwo = String.Format("Паллет: {0} \nуспешно промаркирован!", scanStateFields.strPalletCode),
                           disableCancelButton = true
                       };

                       XtraFormSymbolUserMessage newForm = new XtraFormSymbolUserMessage(newParams);
                       newForm.getLabelControl2().Font = new Font(newForm.getLabelControl2().Font.FontFamily, 36F);
                       newForm.ShowDialog();

                       scanStateFields.currentScanState = enumScanState.ОтсканированДокумент;
                   }                  
               }
               #endregion
            }            
        }

        // метод выполняет сохранение в базе всех результатов сканирования
        private bool lokSaveAllDateToDataBase()
        {
            const string strFuncName = "lokSaveAllDateToDataBase:";
            logger.Trace("Вход в метод {0}. Маркировка по документу {1}",strFuncName, scanStateFields.docTrebovanie);

            //IQueryable<int> rowsListOfDoc = scanStateFields.queryDocRows;
            if (scanStateFields.listActualDocRowIDs == null)
            {
                logger.Error("{0} Маркировка по документу {1}. Ошибка: нет строк документа для сохранения изменений.rowsListOfDoc=null", strFuncName, scanStateFields.docTrebovanie);
                return false;
            }

            // 3.1 установка разделителя по умолчанию для текущего пользователя
            Users current_User = (Users)SecuritySystem.CurrentUser;
            Delimeters delimeter = objSpace.Session().FindObject<Delimeters>(new BinaryOperator("DelimeterID", current_User.DefaultDelimeter.DelimeterID, BinaryOperatorType.Equal), true);

            // 3.2 определение текущего пользователя:
            Users currentUser = objSpace.Session().FindObject<Users>(new BinaryOperator("idd", current_User.idd, BinaryOperatorType.Equal));

            // 3.4 передача в обмен с СУТЗ 7.7 выполненной единицы работы МаркировкаТовара с идентификаторами выполненных строк Спец.прихода:
            XPQuery<JobTypes> queryJobTypes = new XPQuery<JobTypes>(objSpace.Session());
            JobTypes elementJobType = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.ПриемМаркировка select c).FirstOrDefault<JobTypes>();

            // 3.5 поиск текущего документа спец.прихода по идд:
            DocSpecPrihoda currentDocSpecPrih = objSpace.Session().FindObject<DocSpecPrihoda>(new BinaryOperator("idd", scanStateFields.idddocSpecProhoda, BinaryOperatorType.Equal));

            // 3.5 проверка найденных элементов на пустое значение:
            if (currentUser==null)
            {
                logger.Error("{0} Маркировка по документу {1}. Ошибка определения текущего пользователя. currentUser=null", strFuncName, scanStateFields.docTrebovanie);
                return false;
            }
            if (delimeter == null)
            {
                logger.Error("{0} Маркировка по документу {1}. Ошибка определения текущего разделителя учета. delimeter=null", strFuncName, scanStateFields.docTrebovanie);
                return false;
            } 
            if (elementJobType == null)
            {
                logger.Error("{0} Маркировка по документу {1}. Ошибка определения текущего вида работы. elementJobType=null", strFuncName, scanStateFields.docTrebovanie);
                return false;
            }
            if (currentDocSpecPrih == null)
            {
                logger.Error("{0} Маркировка по документу {1}. Ошибка поиск по идд документа спец.прихода. currentDocSpecPrih=null", strFuncName, scanStateFields.docTrebovanie);
                return false;
            }
            // 3.6.1 запись основной единицы работы:
            MobileWorkUnit newMobileUnitMark = new MobileWorkUnit(objSpace.Session()) 
            { 
                BaseDoc = (BaseDocument)currentDocSpecPrih, 
                JobType = elementJobType, 
                AssignedUser = currentUser, 
                TypeOfMobileState = enTypesOfMobileStates.Выполнен, 
                SendedUser = currentUser, 
                Delimeter = delimeter, 
                dTimeInsertUnit = DateTime.Now, 
                idGUID = new Guid(), 
                TypeOfUnitWork = enTypesOfUnitWork.Выполнен,
                Barcode = scanStateFields.palletLabel.Barcode
            };
            newMobileUnitMark.Save();

            // 3.6.2 добавление сотрудника и времени выполнения операции:
            MobileWorkUsers newUserWorkRecord = new MobileWorkUsers(objSpace.Session()) 
            { 
                Delimeter = delimeter, 
                User = currentUser, 
                ErrorState = null, 
                dTimeStart = scanStateFields.dTimeStartOperation, 
                dTimeEnd = DateTime.Now, 
                dTimeClosedTo = scanStateFields.dTimeScanPalletLabel
            };
            newUserWorkRecord.Save();
            newMobileUnitMark.MobileWorkUsersCollection.Add(newUserWorkRecord);

            // 3.6.3 добавление строк документа, по которым была выполнена маркировка:
            XPQuery<DocSpecPrihodaGoods> querySPPrihodaGoods = new XPQuery<DocSpecPrihodaGoods>(objSpace.Session());
            var itemsSPPrihodaGoods = from c in querySPPrihodaGoods where scanStateFields.listActualDocRowIDs.Contains(c.idd) select c;

            foreach (DocSpecPrihodaGoods item in itemsSPPrihodaGoods)
            {
                MobileWorkDocRows newDocRow = new MobileWorkDocRows(objSpace.Session());
                newDocRow.BaseDocRowId = item.idd;
                newDocRow.BaseDocRowGUID = item.DocLineUID;
                newDocRow.idGUID = new Guid();
                newDocRow.Delimeter = delimeter;
                newDocRow.LineNo = item.LineNo;
                newDocRow.Save();
                newMobileUnitMark.MobileWorkDocRowsCollection.Add(newDocRow);
            }            

            // 3.6.4 запись нового статуса паллетной этикетки - промаркирована.
            PalletLabels currentPL = objSpace.Session().FindObject<PalletLabels>(new BinaryOperator("idd", scanStateFields.palletLabel.idd, BinaryOperatorType.Equal)); 
            if (currentPL != null)
            {
                currentPL.PalletLabelStatus = enPalletLabelStatus.ЭтикеткаНаОстатке;
                currentPL.Save();
            }

            // 3.7 нужно выполнить сохранение записанных элементов, чтобы их идд записать в таблицу выгрузки из СУТЗ2:
            if (!lokRunCommitChanges())
            {
                return false;
            }            

            // 3.8.1 добавление единицы работы в задание на выгрузку в 7.7
            NET_SUTZ_Unloads unloads = new NET_SUTZ_Unloads(objSpace.Session());
            unloads.ObjectType = typeof(MobileWorkUnit).FullName;
            unloads.ObjectRow_Id = newMobileUnitMark.idd;
            unloads.Outload = 0;
            unloads.User = currentUser;
            unloads.dTimeInsertRecord = DateTime.Now;
            unloads.dTimeUnloadRecord = null;
            unloads.FullDocTable = false;
            unloads.Save();

            // Подчиненные элементы таблицы будут выгружаться вместе с основной таблицей MobileWorkUnit, поэтому 
            // их не нужно отдельно ставить в задание на выгрузку:
            //// 3.8.2 добавление выполненых строк документа в задание на выгрузку в 7.7
            //foreach (MobileWorkDocRows item in newMobileUnitMark.MobileWorkDocRowsCollection)
            //{
            //    NET_SUTZ_Unloads unloadRow = new NET_SUTZ_Unloads(objSpace.Session());
            //    unloadRow.ObjectType = typeof(MobileWorkDocRows).FullName;
            //    unloadRow.ObjectRow_Id = item.idd;
            //    unloadRow.Outload = 0;
            //    unloadRow.User = currentUser;
            //    unloadRow.dTimeInsertRecord = DateTime.Now;
            //    unloadRow.dTimeUnloadRecord = null;
            //    unloadRow.FullDocTable = false;
            //    unloadRow.Save();
            //}

            //// 3.8.2 добавление сотрудника и времени выполнения в задание на выгрузку в 7.7
            //NET_SUTZ_Unloads unloads1 = new NET_SUTZ_Unloads(objSpace.Session());
            //unloads1.ObjectType = typeof(MobileWorkUsers).FullName;
            //unloads1.ObjectRow_Id = newUserWorkRecord.idd;
            //unloads1.Outload = 0;
            //unloads1.User = currentUser;
            //unloads1.dTimeInsertRecord = DateTime.Now;
            //unloads1.dTimeUnloadRecord = null;
            //unloads1.FullDocTable = false;
            //unloads1.Save();

            // 3.9 передача строки в работу на размещение в моб.сутз:
            elementJobType = (from c in queryJobTypes where c.TypeOfWork == enTypeOfWorks.РазмещениеПрихода select c).FirstOrDefault<JobTypes>();
            if (elementJobType != null)
            {
                MobileWorkUnit newMobileUnit = new MobileWorkUnit(objSpace.Session());
                newMobileUnit.BaseDoc = (BaseDocument)currentDocSpecPrih; //(BaseDocument)scanStateFields.docSpecProhoda;
                newMobileUnit.JobType = elementJobType;
                newMobileUnit.AssignedUser = null;
                newMobileUnit.TypeOfMobileState = enTypesOfMobileStates.ДоступенДляРаботы;
                newMobileUnit.SendedUser = currentUser;
                newMobileUnit.Delimeter = delimeter;
                newMobileUnit.dTimeInsertUnit = DateTime.Now;
                newMobileUnit.idGUID = new Guid();
                newMobileUnit.TypeOfUnitWork = enTypesOfUnitWork.НеОпределен;// пока с заданием не начали работать
                newMobileUnit.Save();

                // 3.7 добавление строк документа, по которым выполняется размещение:
                itemsSPPrihodaGoods = from c in querySPPrihodaGoods where scanStateFields.listActualDocRowIDs.Contains(c.idd) select c;
                foreach (DocSpecPrihodaGoods item in itemsSPPrihodaGoods)
                {
                    MobileWorkDocRows newDocRow = new MobileWorkDocRows(objSpace.Session());
                    newDocRow.BaseDocRowId = item.idd;
                    newDocRow.BaseDocRowGUID = item.DocLineUID;
                    newDocRow.idGUID = new Guid();
                    newDocRow.Delimeter = delimeter;
                    newDocRow.LineNo = item.LineNo;
                    newDocRow.Save();
                    newMobileUnit.MobileWorkDocRowsCollection.Add(newDocRow);
                }                
            }
            else
            {
                logger.Error("{0} При маркировке товара не удалось отправить промаркированный паллет на размещение, т.к. не найден элемент JobTypes с типом 'Размещение'!. SN={1}, спец.прихода={2}.", strFuncName, scanStateFields.strTovarSN, scanStateFields.docSpecProhoda);
            }

            if (!lokRunCommitChanges())
            {
                return false;
            }
            return true;
        }

        private bool lokRunCommitChanges()
        {
            // 1. сохранение данных в транзакции
            // https://documentation.devexpress.com/#CoreLibraries/DevExpressXpoDBExceptions

            String strError = "";

            try
            {
                objSpace.Session().CommitTransaction();
            }
            catch (LockingException lockException)
            {
                strError = String.Format("Ошибка LockingException: {0}", lockException.Message);
                logger.Error(lockException,"lokRunCommitChanges: Ошибка LockingException.");
            }
            catch (SqlExecutionErrorException sqlException)
            {
                strError = String.Format("Ошибка sqlException: {0}", sqlException.Message);
                logger.Error(sqlException,"lokRunCommitChanges: Ошибка sqlException.");

            }
            catch (UnableToOpenDatabaseException openDBException)
            {
                strError = String.Format("Ошибка openDBException: {0}",openDBException.Message);
                logger.Error(openDBException,"lokRunCommitChanges: Ошибка openDBException.");
            }
            catch (Exception ex)
            {
                strError = "Ошибка Exception. " + scanStateFields.docTrebovanie.DocNo;
                logger.Error(ex,"lokRunCommitChanges: Ошибка Exception.");
            }

            if (strError.Length>0)
            {
                // 2. информирование пользователя об ошибке:
                structScanStringParams paramScan = new structScanStringParams() 
                { 
                    captionOne = "Ошибка при сохранении", 
                    captionTwo = strError, 
                    scanedBarcode = "", 
                    successScan = false, 
                    captionHelp = getCurrentOperationStatusString() 
                };

                UserSelect userSelect = new UserSelect();
                userSelect.userMessage(ref paramScan);
                return false;
            }
            return true;
         }

        // локальная структура, для хранения переменных сканирования
        [NonPersistent]
        internal class localClassScanState
        {
            internal JobTypes sprJobeType { get; set; }
            internal BaseDocument docTrebovanie { get; set; }
            internal DocSpecPrihoda docSpecProhoda { get; set; }
            internal int idddocSpecProhoda { get; set; }
            internal Goods goodFinded { get; set; }
            internal DateTime dtTovarDate { get; set; }
            internal String strTovarSN { get; set; }
            internal String strPalletCode { get; set; }
            internal Decimal totalQuantity { get; set; }

            internal PalletLabels palletLabel { get; set; }// сохраняет ссылку на отсканирванную пользователем паллетную этикетку

            // сохраняет идентификаторы строк спец.прихода между запросом сканирования ШК этикетки И методом сохраненения изменения в БД        
            internal List<int> listActualDocRowIDs { get; set; }

            internal List<FreePallet> listActualFreePallets { get; set; }
            internal enumScanState currentScanState { get; set; }
            internal String currentWorkInfo { get; set; }
            // поля для работы логистического блока:
            internal DateTime dTimeStartOperation { get; set; }
            internal DateTime dTimeScanPalletLabel { get; set; }

            // количество оставшихся для маркировки паллет:
            internal UInt16 totalNotMarkedPallets { get; set; }
        }

    }
}
