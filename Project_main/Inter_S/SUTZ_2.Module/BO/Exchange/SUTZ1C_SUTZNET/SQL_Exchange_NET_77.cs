using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using NLog;
using DevExpress.ExpressApp.Utils;
using SUTZ_2.Module.BO;
using SUTZ_2.Module.BO.Documents;
using SUTZ_2.Module.BO.References;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using System.Collections;
using DevExpress.ExpressApp.Xpo;
using SUTZ_2.Module.BO.References.Mobile;
using System.Threading.Tasks;

namespace SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET
{
    [NavigationItem("Обработки"), ImageName("BO_Report")]
    [XafDisplayName("Выгрузка из NET.СУТЗ в 1С:7.7")]
    [NonPersistent]
    class SQL_Exchange_NET_77
    {
        [VisibleInDetailView(false), VisibleInListView(false)]
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // не визуальные компоненты
        [VisibleInDetailView(false), VisibleInListView(false)]
        private IObjectSpace objectSpace; 
        
        public Action<String, String> delegateForShowMessage;
        public Action<Type, int> delegateForCalculateItems;

        public SQL_Exchange_NET_77()
        {

        }
        // метод передает сигнал на изменение счетчика выгруженных объектов определенного типа:
        private void lokSendMessageForObjectType(Type objType, int rowCounts)
        {
            if (delegateForCalculateItems!=null)
            {
                delegateForCalculateItems(objType, rowCounts);
            }            
        }

        // метод выводит сообщения в форму и в логи.
        // Передаваемые параметры: "strMessage" - сообщение;
        //                          traceLevel - строка, уровень сообщения.
        private void lokSendAndShowMessage(String strMessage, string traceLevel, Exception ex){
            delegateForShowMessage(strMessage, traceLevel);
            if (traceLevel.Equals("Trace"))
            {
                logger.Trace(strMessage); 
            }
            else if (traceLevel.Equals("Debug"))
            {
                logger.Debug(strMessage); 
            }
            else if (traceLevel.Equals("Warning"))
            {
                logger.Warn(strMessage); 
            }
            else if (traceLevel.Equals("Exception"))
            {
                logger.Trace(ex, "Ошибка при вызове CommitChanges, message={0}", strMessage);
            }               
        }

        public SQL_Exchange_NET_77(IObjectSpace objSpace): this()
        {
            Guard.ArgumentNotNull(objSpace, "objSpace");
            objectSpace = objSpace;
        }

        // общий метод для выгрузки всех данных в 7.7
        public void runOutboundAllDate()
        {
            bool result = false;

            // 1. выборка всех объектов для выгрузки:
            XPQuery<NET_SUTZ_Unloads> records = new XPQuery<NET_SUTZ_Unloads>(objectSpace.Session());            
            var listOfRecords = from c in records orderby c.row_id where c.Outload == 0 select c;

            // 2. нужно получить все строки одного объекта, присутствующие в выгрузке
            
            foreach (NET_SUTZ_Unloads item in listOfRecords)
            {
                result = false;

                // 1.1 проверка на ошибки:
                if (item.ObjectType.Length==0)
                {
                    // почему-то тип объекта пустой, отметим строку как выгруженную
                    item.Outload = 13;
                    item.dTimeUnloadRecord = DateTime.Now;
                    item.Save();
                }

               // 1.2 получение типа данных из строки:
               Type tekType = null;
               try
               {
	                tekType = Type.GetType(item.ObjectType);
               }
               catch (System.Exception ex)
               {
                   item.Outload = 14;
                   item.dTimeUnloadRecord = DateTime.Now;
                   lokSendAndShowMessage(String.Format("Ошибка при попытке получить тип из строки {0}, ошибка: ex={1}",  ex.Message, ex), "Exception", ex);
                   continue;
               }

                // 2. выгрузка документа Спецификация прихода:
               if (tekType == typeof(DocSpecPrihoda))
               {
                   result = outboundSpecPrihoda(item);
                   if (result)
                   {
                       String strMessage = String.Format("Успешно выгружен DocSpecPrihoda: row_id net_sutz_unloads:{0}", item.row_id);
                       lokSendAndShowMessage(strMessage, "Trace", null);
                       lokSendMessageForObjectType(typeof(NET_SUTZ_DocSpecPrihodaHeaders), 1);
                   }

               }
               else if (tekType == typeof(DocSpecPeremeshenie))
               {
                   result = outboundSpecPeremesh(item);
                   if (result)
                   {
                       String strMessage = String.Format("Успешно выгружен DocSpecPeremesh: row_id net_sutz_unloads:{0}", item.row_id);
                       lokSendAndShowMessage(strMessage, "Trace", null);
                       lokSendMessageForObjectType(typeof(NET_SUTZ_DocSpecPeremeshHeaders), 1);
                   }
               }
               else if (tekType == typeof(DocSpecRashoda))
               {
                   result = outboundSpecRash(item);
                   if (result)
                   {
                       String strMessage = String.Format("Успешно выгружен DocSpecRash: row_id net_sutz_unloads:{0}", item.row_id);
                       lokSendAndShowMessage(strMessage, "Trace", null);
                       lokSendMessageForObjectType(typeof(NET_SUTZ_DocSpecRashHeaders), 1);
                   }
               }
               else if (tekType == typeof(MobileWorkUnit))
               {
                   result = outboundMobileWorkUnit(item);
                   if (result)
                   {
                       String strMessage = String.Format("Успешно выгружен MobileWorkUnit: row_id net_sutz_unloads:{0}", item.row_id);
                       lokSendAndShowMessage(strMessage, "Trace", null);
                       lokSendMessageForObjectType(typeof(NET_SUTZ_DocJobeProperties), 1);
                   }
               }
               else if (tekType == typeof(DocJobesProperties))
               {
                   result = outboundDocJobeProperties(item);
               }

               if (result)
               {
                   item.Outload = 1;
                   item.dTimeUnloadRecord = DateTime.Now;
                   item.Save();
               }

               // запись изменений в               
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    try
                    {
                        objectSpace.CommitChanges();
                    }
                    catch (System.Exception ex)
                    {
                        lokSendAndShowMessage(String.Format("Ошибка при вызове CommitChanges(), документ Спецификация перемещения. ex={0}", ex.Message), "Exception", ex);
                    } 
                }
            }
            try
            {
                objectSpace.CommitChanges();
            }
            catch (System.Exception ex)
            {
                lokSendAndShowMessage(String.Format("Ошибка при вызове CommitChanges(), документ Спецификация перемещения. ex={0}", ex.Message), "Exception", ex);
            } 
        }

        private bool outboundDocJobeProperties(NET_SUTZ_Unloads item)
        {
            DocJobesProperties elementFinded = objectSpace.Session().GetObjectByKey<DocJobesProperties>(item.ObjectRow_Id);
            if (elementFinded == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при поиске элемента DocJobeProperties по id={0}", item.ObjectRow_Id), "Warning", null);
                return false;
            }

            // 1.1 проверка заполнения полей элемента выгрузки:
            if (elementFinded.JobType == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при выгрузке элемента DocJobeProperties: не заполнено поле JobeType! DocJobeProperties.idd={0}", elementFinded.idd), "Warning", null);
                return false;
            }
            if (elementFinded.BaseDoc == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при выгрузке элемента DocJobeProperties: не заполнено поле BaseDoc! DocJobeProperties.idd={0}", elementFinded.idd), "Warning", null);
                return false;
            }

            if (elementFinded.TypeOfMobileState == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при выгрузке элемента DocJobeProperties: не заполнено поле TypeOfMobileState! DocJobeProperties.idd={0}", elementFinded.idd), "Warning", null);
                return false;
            }
            // 2. добавление заголовочной строки в таблицу шапки выгрузки:
            NET_SUTZ_DocJobeProperties newElementOut = new NET_SUTZ_DocJobeProperties(objectSpace.Session());
            newElementOut.iddSUTZ_GUID = (Guid)elementFinded.idGUID;
            newElementOut.Delimeter = elementFinded.Delimeter.DelimeterID;
            newElementOut.JobeType = elementFinded.JobType.idGUID;
            newElementOut.DocBaseID = elementFinded.BaseDoc.idGUID;
            newElementOut.MobileStatus = (byte)elementFinded.TypeOfMobileState;
            newElementOut.dTimeWrite = DateTime.Now;
            newElementOut.UserHostWriter = (SUTZ_NET_HostsUsers)objectSpace.GetObjectByKey(typeof(SUTZ_NET_HostsUsers), currentSessionSettings.CurrentHostUser.row_id);// currentSessionSettings.CurrentHostUser;
            newElementOut.is_read = 0;
            newElementOut.Save();
            return true;
        }

        // метод для выгрузки единицы работы
        private bool outboundMobileWorkUnit(NET_SUTZ_Unloads item)
        {
            // 1. поиск документа в базе 
            MobileWorkUnit elementFinded = (objectSpace.Session().GetObjectByKey<MobileWorkUnit>(item.ObjectRow_Id));
            if (elementFinded == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при поиске элемента MobileWorkUnit по id={0}", item.ObjectRow_Id), "Warning", null);
                return false;
            }

            // 1.1 проверка заполнения полей элемента выгрузки:
            if (elementFinded.JobType == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при выгрузке элемента MobileWorkUnit: не заполнено поле JobeType! MobileWorkUnit.idd={0}", elementFinded.idd), "Warning", null);
                return false;
            }
            if (elementFinded.BaseDoc == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при выгрузке элемента MobileWorkUnit: не заполнено поле BaseDoc! MobileWorkUnit.idd={0}", elementFinded.idd), "Warning", null);
                return false;
            }
            // 2. добавление заголовочной строки в таблицу шапки выгрузки:
            NET_SUTZ_MobileWorkUnit newElementHeaderOut = new NET_SUTZ_MobileWorkUnit(objectSpace.Session());
            //newElementHeaderOut.iddSUTZ_guid = (Guid)docFinded.idGUID;
            newElementHeaderOut.JobeTypeID = elementFinded.JobType.idGUID;
            newElementHeaderOut.DocBaseID = elementFinded.BaseDoc.idGUID;
            newElementHeaderOut.dTimeWrite = DateTime.Now;
            newElementHeaderOut.UserHostWriter = (SUTZ_NET_HostsUsers)objectSpace.GetObjectByKey(typeof(SUTZ_NET_HostsUsers), currentSessionSettings.CurrentHostUser.row_id);// currentSessionSettings.CurrentHostUser;
            newElementHeaderOut.is_read = 0;

            // 2.1 добавление выполненных строк документа
            foreach (MobileWorkDocRows itemDocRow in elementFinded.MobileWorkDocRowsCollection)
	        {
                NET_SUTZ_MobileWorkDocRows newElementDocRow = new NET_SUTZ_MobileWorkDocRows(objectSpace.Session()) 
                { 
                    DocLineGUID = itemDocRow.BaseDocRowGUID, 
                    LineNo = itemDocRow.LineNo 
                };
                newElementHeaderOut.NET_SUTZ_MobileWorkDocRows.Add(newElementDocRow);
	        }         

            // 2.2 добавление сотрудников задания
            foreach (MobileWorkUsers itemDocUser in elementFinded.MobileWorkUsersCollection)
            {
                NET_SUTZ_MobileWorkUsers newElementDocUser = new NET_SUTZ_MobileWorkUsers(objectSpace.Session()) 
                { 
                    User = itemDocUser.User.idGUID, 
                    dTimeStart = itemDocUser.dTimeStart, 
                    dTimeEnd = itemDocUser.dTimeEnd, 
                    iddint = itemDocUser.idd,
                    MobileError = (itemDocUser.ErrorState!=null ? itemDocUser.ErrorState.idGUID : Guid.Empty)
                };

                newElementHeaderOut.NET_SUTZ_MobileWorkUsers.Add(newElementDocUser);
            }

            newElementHeaderOut.Save();
            return true;
        }

        // 1. Выгрузка спецификации прихода
        private bool outboundSpecPrihoda(NET_SUTZ_Unloads item)
        {
            // 1. поиск документа в базе 
            DocSpecPrihoda docFinded = (DocSpecPrihoda)(objectSpace.Session().GetObjectByKey(typeof(DocSpecPrihoda), item.ObjectRow_Id));
            if (docFinded==null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при поиске документа Спец.прихода по id={0}", item.ObjectRow_Id),"Warning",null);
                return false;
            }

            // 2. добавление заголовочной строки в таблицу шапки выгрузки:
            NET_SUTZ_DocSpecPrihodaHeaders newDocSpPr = new NET_SUTZ_DocSpecPrihodaHeaders(objectSpace.Session());
            newDocSpPr.iddSUTZ_guid = (Guid)docFinded.idGUID;
            newDocSpPr.DocNo        = docFinded.DocNo;
            newDocSpPr.dTimeWrite   = DateTime.Now;
            newDocSpPr.UserHostWriter = (SUTZ_NET_HostsUsers)objectSpace.GetObjectByKey(typeof(SUTZ_NET_HostsUsers), currentSessionSettings.CurrentHostUser.row_id);// currentSessionSettings.CurrentHostUser;
            newDocSpPr.is_read      = 0;
            newDocSpPr.FullDocTable = item.FullDocTable;

            // 3. Определение, сколько строк выгружается - одна или весь документ.
            if (item.FullDocTable)
            {
                foreach (DocSpecPrihodaGoods DocRowItem in docFinded.DocListOfGoods)
                {
                    NET_SUTZ_DocSpecPrihodaGoods newDocRowItem = new NET_SUTZ_DocSpecPrihodaGoods(objectSpace.Session());//((XPObjectSpace)objectSpace).Session
                    newDocRowItem.DocLineNo = DocRowItem.LineNo;
                    newDocRowItem.GoodID = DocRowItem.Good.idGUID;
                    newDocRowItem.UnitID = DocRowItem.Unit.idGUID;
                    newDocRowItem.QuantityOfUnits = DocRowItem.QuantityOfItems;
                    newDocRowItem.QuantityOfItems = DocRowItem.QuantityOfItems;
                    newDocRowItem.TotalQuantity = DocRowItem.TotalQuantity;
                    newDocRowItem.TakeFrom = DocRowItem.TakeFrom.idGUID;
                    newDocRowItem.PutTo = DocRowItem.PutTo.idGUID;
                    newDocRowItem.BestBefore = DocRowItem.BestBefore;
                    newDocRowItem.PropertyID = DocRowItem.Property.idGUID;
                    newDocRowItem.DocLineUID = DocRowItem.DocLineUID;
                    newDocRowItem.QuantityFact = DocRowItem.QuantityFact;
                    //newDocRowItem.QuantityOrig = DocRowItem.QuantityOrig;

                    newDocSpPr.NET_SUTZ_DocSpecPrihodaGoods.Add(newDocRowItem);
                }
            }
            else//3.1 выгружается одна или несколько строк документа
            {
                foreach (NET_SUTZ_DocRows itemDocRow in item.NET_SUTZ_DocRows)
                {
                    DocSpecPrihodaGoods currentDocRow = null;
                    try
                    {
                        currentDocRow = docFinded.DocListOfGoods.Lookup(itemDocRow.ObjectDocTableId);
                    }
                    catch (System.Exception ex)
                    {
                        lokSendAndShowMessage(String.Format("Ошибка при поиске строки с idd={0} документа Спец.прихода id={1}, ошибка:{2}", itemDocRow.ObjectDocTableId, docFinded.DocNo, ex), "Warning", null);
                    }

                    NET_SUTZ_DocSpecPrihodaGoods newDocRowItem = new NET_SUTZ_DocSpecPrihodaGoods(objectSpace.Session())
                    { 
                        DocLineNo = currentDocRow.LineNo, 
                        DocLineUID = currentDocRow.DocLineUID
                    };

                    // 3.1.3 проверим необходимость выгрузки полей строки документа по битовым флагам:
                    // Товар:

                    if (itemDocRow.BitFlagsOfOuboundFields.HasFlag(enFlagsOfOuboundFields.Товар))
                    {
                        newDocRowItem.GoodID = currentDocRow.Good.idGUID;
                    }
                    // Единица измерения:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ЕдиницаИзмерения) == enFlagsOfOuboundFields.ЕдиницаИзмерения)
                    {
                        newDocRowItem.UnitID = currentDocRow.Unit.idGUID;
                    }
                    // Количество коробок:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоКор) == enFlagsOfOuboundFields.КоличествоКор)
                    {
                        newDocRowItem.QuantityOfUnits = currentDocRow.QuantityOfUnits;
                    }
                    // Количество штук:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоШт) == enFlagsOfOuboundFields.КоличествоШт)
                    {
                        newDocRowItem.QuantityOfItems = currentDocRow.QuantityOfItems;
                    }
                    // Общее количество:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.Количество) == enFlagsOfOuboundFields.Количество)
                    {
                        newDocRowItem.TotalQuantity = currentDocRow.TotalQuantity;
                    }
                    // Статус проведения строки документа:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.СтатусПроведения) == enFlagsOfOuboundFields.СтатусПроведения)
                    {
                        newDocRowItem.MoveStatus = currentDocRow.MoveStatus;
                    }
                    // Ячейка положить В:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ПоложитьВ) == enFlagsOfOuboundFields.ПоложитьВ)
                    {
                        newDocRowItem.PutTo = currentDocRow.PutTo.idGUID;
                    }
                    // Срок годности товара:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.СрокГодности) == enFlagsOfOuboundFields.СрокГодности)
                    {
                        newDocRowItem.BestBefore = currentDocRow.BestBefore;
                    }

                    // Характеристика товара:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.Характеристика) == enFlagsOfOuboundFields.Характеристика)
                    {
                        if (currentDocRow.Property!=null)
                        {
                            newDocRowItem.PropertyID = currentDocRow.Property.idGUID;
                        }                        
                    }
                    
                    // Взять из, для документа размещения из кладовой приема:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ВзятьИз) == enFlagsOfOuboundFields.ВзятьИз)
                    {
                        if (currentDocRow.TakeFrom!=null)
                        {
                            newDocRowItem.TakeFrom = currentDocRow.TakeFrom.idGUID;
                        }                        
                    }

                    // ТочкаПередачи, для документа 
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ТочкаПередачи) == enFlagsOfOuboundFields.ТочкаПередачи)
                    {
                        if (currentDocRow.ExchangePoint!=null)
                        {
                            newDocRowItem.TransferPointID = currentDocRow.ExchangePoint.idGUID;
                        }                        
                    }
                    newDocSpPr.NET_SUTZ_DocSpecPrihodaGoods.Add(newDocRowItem);
                }           
            }
            newDocSpPr.Save();
            return true;
        }

        // 2. Выгрузка спецификации перемещения
        private bool outboundSpecPeremesh(NET_SUTZ_Unloads item)
        {
            const string funcName = "outboundSpecPeremesh";
            // 1. поиск документа в базе 
            DocSpecPeremeshenie docFinded = (DocSpecPeremeshenie)(objectSpace.Session().GetObjectByKey(typeof(DocSpecPeremeshenie), item.ObjectRow_Id));
            if (docFinded == null)
            {
                lokSendAndShowMessage(String.Format("{0} Ошибка при поиске документа Спец.перемещение по id={1}", funcName, item.ObjectRow_Id), "Warning", null);
                return false;
            }

            // 2. добавление заголовочной строки в таблицу шапки выгрузки:
            NET_SUTZ_DocSpecPeremeshHeaders newDocOut = new NET_SUTZ_DocSpecPeremeshHeaders(objectSpace.Session());
            newDocOut.iddSUTZ_guid = (Guid)docFinded.idGUID;
            newDocOut.DocNo = docFinded.DocNo;
            newDocOut.dTimeWrite = DateTime.Now;
            newDocOut.UserHostWriter = (SUTZ_NET_HostsUsers)objectSpace.GetObjectByKey(typeof(SUTZ_NET_HostsUsers), currentSessionSettings.CurrentHostUser.row_id);// currentSessionSettings.CurrentHostUser;
            newDocOut.is_read = 0;
            newDocOut.FullDocTable = item.FullDocTable;
            newDocOut.AllowMovement = docFinded.AllowMovement;

            // 3. Определение, сколько строк выгружается - одна или весь документ.
            if (item.FullDocTable)
            {
                foreach (DocSpecPeremeshGoods DocRowItem in docFinded.DocListOfGoods)
                {
                    NET_SUTZ_DocSpecPeremeshGoods newDocRowItem = new NET_SUTZ_DocSpecPeremeshGoods(objectSpace.Session()) 
                    { 
                        DocLineNo = DocRowItem.LineNo, 
                        GoodID = DocRowItem.Good.idGUID, 
                        UnitID = DocRowItem.Unit.idGUID, 
                        QuantityOfUnits = DocRowItem.QuantityOfItems, 
                        QuantityOfItems = DocRowItem.QuantityOfItems, 
                        TotalQuantity = DocRowItem.TotalQuantity, 
                        QuantityFact = DocRowItem.QuantityFact,
                        TakeFrom = DocRowItem.TakeFrom.idGUID, 
                        PutTo = DocRowItem.PutTo.idGUID, 
                        BestBefore = DocRowItem.BestBefore, 
                        PropertyID = DocRowItem.Property.idGUID, 
                        DocLineUID = DocRowItem.DocLineUID 
                    };
                    newDocOut.NET_SUTZ_DocSpecPeremeshGoods.Add(newDocRowItem);
                }
            }
            else//3.1 выгружается одна или несколько строк документа
            {
                foreach (NET_SUTZ_DocRows itemDocRow in item.NET_SUTZ_DocRows)
                {
                    DocSpecPeremeshGoods currentDocRow = null;
                    try
                    {
                        currentDocRow = docFinded.DocListOfGoods.Lookup(itemDocRow.ObjectDocTableId);
                    }
                    catch (System.Exception ex)
                    {
                        lokSendAndShowMessage(String.Format("{0}: Ошибка при поиске строки с idd={1} документа Спец.перемещения id={2}",funcName, itemDocRow.ObjectDocTableId, docFinded.DocNo), "Warning", null);
                    }

                    NET_SUTZ_DocSpecPeremeshGoods newDocRowItem = new NET_SUTZ_DocSpecPeremeshGoods(objectSpace.Session())
                    {
                        DocLineNo = currentDocRow.LineNo,
                        DocLineUID = currentDocRow.DocLineUID
                    };

                    // 3.1.3 проверим необходимость выгрузки полей строки документа по битовым флагам:
                    // Товар:
                    if (itemDocRow.BitFlagsOfOuboundFields.HasFlag(enFlagsOfOuboundFields.Товар))
                    {
                        newDocRowItem.GoodID = currentDocRow.Good.idGUID;
                    }
                    // Единица измерения:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ЕдиницаИзмерения) == enFlagsOfOuboundFields.ЕдиницаИзмерения)
                    {
                        newDocRowItem.UnitID = currentDocRow.Unit.idGUID;
                    }
                    // Количество коробок:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоКор) == enFlagsOfOuboundFields.КоличествоКор)
                    {
                        newDocRowItem.QuantityOfUnits = currentDocRow.QuantityOfUnits;
                    }
                    // Количество штук:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоШт) == enFlagsOfOuboundFields.КоличествоШт)
                    {
                        newDocRowItem.QuantityOfItems = currentDocRow.QuantityOfItems;
                    }
                    // Общее количество:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.Количество) == enFlagsOfOuboundFields.Количество)
                    {
                        newDocRowItem.TotalQuantity = currentDocRow.TotalQuantity;
                    }
                    // Количество факт:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоФакт) == enFlagsOfOuboundFields.КоличествоФакт)
                    {
                        newDocRowItem.QuantityFact = currentDocRow.QuantityFact;
                    }
                    // Статус проведения строки документа:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.СтатусПроведения) == enFlagsOfOuboundFields.СтатусПроведения)
                    {
                        newDocRowItem.MoveStatus = currentDocRow.MoveStatus;
                    }
                    // Ячейка положить В:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ПоложитьВ) == enFlagsOfOuboundFields.ПоложитьВ)
                    {
                        newDocRowItem.PutTo = currentDocRow.PutTo.idGUID;
                    }
                    // Срок годности товара:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.СрокГодности) == enFlagsOfOuboundFields.СрокГодности)
                    {
                        newDocRowItem.BestBefore = currentDocRow.BestBefore;
                    }

                    // Характеристика товара:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.Характеристика) == enFlagsOfOuboundFields.Характеристика)
                    {
                        if (currentDocRow.Property != null)
                        {
                            newDocRowItem.PropertyID = currentDocRow.Property.idGUID;
                        }
                    }

                    // Взять из, для документа размещения из кладовой приема:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ВзятьИз) == enFlagsOfOuboundFields.ВзятьИз)
                    {
                        if (currentDocRow.TakeFrom != null)
                        {
                            newDocRowItem.TakeFrom = currentDocRow.TakeFrom.idGUID;
                        }
                    }

                    // ТочкаПередачи, для документа 
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ТочкаПередачи) == enFlagsOfOuboundFields.ТочкаПередачи)
                    {
                        if (currentDocRow.ExchangePoint != null)
                        {
                            newDocRowItem.TransferPointID = currentDocRow.ExchangePoint.idGUID;
                        }
                    }
                    newDocOut.NET_SUTZ_DocSpecPeremeshGoods.Add(newDocRowItem);
                }
            }
            newDocOut.Save();
            return true;
        }

        // 2. Выгрузка спецификации перемещения
        private bool outboundSpecRash(NET_SUTZ_Unloads item)
        {
            const string funcName = "outboundSpecRash";
            // 1. поиск документа в базе 
            DocSpecRashoda docFinded = (DocSpecRashoda)(objectSpace.Session().GetObjectByKey(typeof(DocSpecRashoda), item.ObjectRow_Id));
            if (docFinded == null)
            {
                lokSendAndShowMessage(String.Format("{0} Ошибка при поиске документа Спец.расхода по id={1}", funcName, item.ObjectRow_Id), "Warning", null);
                return false;
            }

            // 2. добавление заголовочной строки в таблицу шапки выгрузки:
            NET_SUTZ_DocSpecRashHeaders newDocOut = new NET_SUTZ_DocSpecRashHeaders(objectSpace.Session());
            newDocOut.iddSUTZ_guid = (Guid)docFinded.idGUID;
            newDocOut.DocNo = docFinded.DocNo;
            newDocOut.dTimeWrite = DateTime.Now;
            newDocOut.UserHostWriter = (SUTZ_NET_HostsUsers)objectSpace.GetObjectByKey(typeof(SUTZ_NET_HostsUsers), currentSessionSettings.CurrentHostUser.row_id);// currentSessionSettings.CurrentHostUser;
            newDocOut.is_read = 0;
            newDocOut.FullDocTable = item.FullDocTable;
            newDocOut.TovarOtobran = docFinded.TovarOtobran;
            newDocOut.TovarPoluchen = docFinded.TovarPoluchen;

            // 3. Определение, сколько строк выгружается - одна или весь документ.
            if (item.FullDocTable)
            {
                foreach (DocSpecRashodaGoods DocRowItem in docFinded.DocListOfGoods)
                {
                    NET_SUTZ_DocSpecRashGoods newDocRowItem = new NET_SUTZ_DocSpecRashGoods(objectSpace.Session())
                    {
                        DocLineNo = DocRowItem.LineNo,
                        GoodID = DocRowItem.Good.idGUID,
                        UnitID = DocRowItem.Unit.idGUID,
                        QuantityOfUnits = DocRowItem.QuantityOfItems,
                        QuantityOfItems = DocRowItem.QuantityOfItems,
                        TotalQuantity = DocRowItem.TotalQuantity,
                        QuantityFact = DocRowItem.QuantityFact,
                        TakeFrom = DocRowItem.TakeFrom.idGUID,
                        PutTo = DocRowItem.PutTo.idGUID,
                        BestBefore = DocRowItem.BestBefore,
                        PropertyID = DocRowItem.Property.idGUID,
                        DocLineUID = DocRowItem.DocLineUID
                    };
                    newDocOut.NET_SUTZ_DocSpecRashGoods.Add(newDocRowItem);
                }
            }
            else//3.1 выгружается одна или несколько строк документа
            {
                foreach (NET_SUTZ_DocRows itemDocRow in item.NET_SUTZ_DocRows)
                {
                    DocSpecRashodaGoods currentDocRow = null;
                    try
                    {
                        currentDocRow = docFinded.DocListOfGoods.Lookup(itemDocRow.ObjectDocTableId);
                    }
                    catch (System.Exception ex)
                    {
                        lokSendAndShowMessage(String.Format("{0}: Ошибка при поиске строки с idd={1} документа Спец.расхода id={2}", funcName, itemDocRow.ObjectDocTableId, docFinded.DocNo), "Warning", null);
                    }

                    NET_SUTZ_DocSpecRashGoods newDocRowItem = new NET_SUTZ_DocSpecRashGoods(objectSpace.Session())
                    {
                        DocLineNo = currentDocRow.LineNo,
                        DocLineUID = currentDocRow.DocLineUID
                    };

                    // 3.1.3 проверим необходимость выгрузки полей строки документа по битовым флагам:
                    // Товар:
                    if (itemDocRow.BitFlagsOfOuboundFields.HasFlag(enFlagsOfOuboundFields.Товар))
                    {
                        newDocRowItem.GoodID = currentDocRow.Good.idGUID;
                    }
                    // Единица измерения:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ЕдиницаИзмерения) == enFlagsOfOuboundFields.ЕдиницаИзмерения)
                    {
                        newDocRowItem.UnitID = currentDocRow.Unit.idGUID;
                    }
                    // Количество коробок:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоКор) == enFlagsOfOuboundFields.КоличествоКор)
                    {
                        newDocRowItem.QuantityOfUnits = currentDocRow.QuantityOfUnits;
                    }
                    // Количество штук:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоШт) == enFlagsOfOuboundFields.КоличествоШт)
                    {
                        newDocRowItem.QuantityOfItems = currentDocRow.QuantityOfItems;
                    }
                    // Общее количество:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.Количество) == enFlagsOfOuboundFields.Количество)
                    {
                        newDocRowItem.TotalQuantity = currentDocRow.TotalQuantity;
                    }
                    // Количество факт:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.КоличествоФакт) == enFlagsOfOuboundFields.КоличествоФакт)
                    {
                        newDocRowItem.QuantityFact = currentDocRow.QuantityFact;
                    }
                    // Статус проведения строки документа:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.СтатусПроведения) == enFlagsOfOuboundFields.СтатусПроведения)
                    {
                        newDocRowItem.MoveStatus = currentDocRow.MoveStatus;
                    }
                    // Ячейка положить В:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ПоложитьВ) == enFlagsOfOuboundFields.ПоложитьВ)
                    {
                        newDocRowItem.PutTo = currentDocRow.PutTo.idGUID;
                    }
                    // Срок годности товара:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.СрокГодности) == enFlagsOfOuboundFields.СрокГодности)
                    {
                        newDocRowItem.BestBefore = currentDocRow.BestBefore;
                    }

                    // Характеристика товара:
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.Характеристика) == enFlagsOfOuboundFields.Характеристика)
                    {
                        if (currentDocRow.Property != null)
                        {
                            newDocRowItem.PropertyID = currentDocRow.Property.idGUID;
                        }
                    }
                    
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ВзятьИз) == enFlagsOfOuboundFields.ВзятьИз)
                    {
                        if (currentDocRow.TakeFrom != null)
                        {
                            newDocRowItem.TakeFrom = currentDocRow.TakeFrom.idGUID;
                        }
                    }

                    // ТочкаПередачи, для документа 
                    if ((itemDocRow.BitFlagsOfOuboundFields & enFlagsOfOuboundFields.ТочкаПередачи) == enFlagsOfOuboundFields.ТочкаПередачи)
                    {
                        if (currentDocRow.ExchangePoint != null)
                        {
                            newDocRowItem.TransferPointID = currentDocRow.ExchangePoint.idGUID;
                        }
                    }
                    newDocOut.NET_SUTZ_DocSpecRashGoods.Add(newDocRowItem);
                }
            }
            newDocOut.Save();
            return true;
        }

        // 1. нужно получить список всех объектов для выгрузки:
        public int calculateAllOutBoundRows()
        {            
            int totalItems = new XPQuery<NET_SUTZ_Unloads>(objectSpace.Session()).Count(c => c.Outload==0); 
            return totalItems;
        }
    }
}
