using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using System.Collections;
using DevExpress.ExpressApp.Xpo;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO.Documents;
using SUTZ_2.Module.BO.References.Mobile;
using DevExpress.ExpressApp.Utils;
using NLog;
using SUTZ_2.Module.BO.GLThreadings;
using System.Threading.Tasks;
using System.Threading;
using SUTZ_2.Module.BO.References.Mobile.Modules;

namespace SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET
{
   // [DomainComponent]
    [NavigationItem("Обработки"), ImageName("BO_Report")]
    [XafDisplayName("Загрузка из СУТЗ 1С:7.7")]
    [NonPersistent]
    public class SQL_Exchange_Inbound: INotifyPropertyChanged
    {
        [VisibleInDetailView(false), VisibleInListView(false)]
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // не визуальные компоненты
        [VisibleInDetailView(false),VisibleInListView(false)]
        private IObjectSpace objectSpace;

        // конструктор с делегатом для отправки сообщения
        public Action<String, String> delegateForShowMessage;
        public Action<Type, int> delegateForCalculateItems;

        // токен для отмены операции
        CancellationToken clToken;  

        // визуальные компоненты:
        #region ВизуальныеФлагиВключенияВыключенияЗагрузкиОбъектов
      // 1. загрука справочника Номенклатуры
        private bool isLoadGoods_;
        public bool isLoadGoods
        {
            get { return isLoadGoods_; }
            set { isLoadGoods_ = value; }
        }        

        // 2. загрузка справочника Единиц измерения
        private bool isLoadUnitsOfGoods;
        public bool IsLoadUnitOfGoods
        {
            get { return isLoadUnitsOfGoods; }
            set { isLoadUnitsOfGoods = value; }
        }        

        // 3. загружаем справочник Кладовых
        private bool isLoadStockRooms;
        public bool IsLoadStockRooms
        {
            get { return isLoadStockRooms; }
            set { isLoadStockRooms = value; }
        }

        // 4. Загрузка справочника Кодов размещения;
        private bool isLoadStorageCodes;
        public bool IsLoadStorageCodes
        {
            get { return isLoadStorageCodes; }
            set { isLoadStorageCodes = value; }
        }
        
        // 5. Загрузка справочника Характеристик номенклатуры
        private bool isLoadProperties;
        public bool IsLoadProperties
        {
            get { return isLoadProperties; }
            set { isLoadProperties = value; }
        }
        
        // 6. Загрузка справочика Страны
        private bool isLoadCountries;
        public bool IsLoadCountries
        {
            get { return isLoadCountries; }
            set { isLoadCountries = value; }
        }
        
        // 7. Загрузка справочника Производители
        private bool isLoadManufacturers;
        public bool IsLoadManufacturers
        {
            get { return isLoadManufacturers; }
            set { isLoadManufacturers = value; }
        }

        // 8. Загрузка справочника ОКЕИ
        private bool isLoadOKEI;
        public bool IsLoadOKEI
        {
            get { return isLoadOKEI; }
            set { isLoadOKEI = value; }
        }

        // 9. Загрузка справочника Контрагентов
        private bool isLoadClients;
        public bool IsLoadClients
        {
            get { return isLoadClients; }
            set { isLoadClients = value; }
        }

        // 9. Загрузка справочника ВидовРабот
        private bool isLoadJobTypes;
        public bool IsLoadJobTypes
        {
            get { return isLoadJobTypes; }
            set { isLoadJobTypes = value; }
        }

        // 9. Загрузка справочника ВидовРабот
        private bool isLoadBarcodesOfGoods;
        public bool IsLoadBarcodesOfGoods
        {
            get { return isLoadBarcodesOfGoods; }
            set { isLoadBarcodesOfGoods = value; }
        }

        // 9. Загрузка справочника Логистика
        private bool isLoadLogistics;
        public bool IsLoadLogistics
        {
            get { return isLoadLogistics; }
            set { isLoadLogistics = value; }
        }

        // 10. Загрузка справочника Подразделения контрагента
        private bool isLoadCustomerDivisions;
        public bool IsLoadCustomerDivisions
        {
            get { return isLoadCustomerDivisions; }
            set { isLoadCustomerDivisions = value; }
        }

        // 11. Загрузка справочника Магазины контрагента
        private bool isLoadCustomerStores;
        public bool IsLoadCustomerStores
        {
            get { return isLoadCustomerStores; }
            set { isLoadCustomerStores = value; }
        }

        // 12. Загрузка справочника Магазины контрагента
        private bool isLoadWarehouses;
        public bool IsLoadWarehouses
        {
            get { return isLoadWarehouses; }
            set { isLoadWarehouses = value; }
        }

        // 13. Загрузка справочника Пользователи
        private bool isLoadUsers;
        public bool IsLoadUsers
        {
            get { return isLoadUsers; }
            set { isLoadUsers = value; }
        }

        // 14. загрузка справочника СвойствДокумента
        private bool isLoadDocProperties;
        public bool IsLoadDocProperties
        {
            get { return isLoadDocProperties; }
            set { isLoadDocProperties = value; }
        }

        // 15. загрузка справочника СвойствДокумента
        private bool isLoadDocJobeProperties;
        public bool IsLoadDocJobeProperties
        {
            get { return isLoadDocJobeProperties; }
            set { isLoadDocJobeProperties = value; }
        }

        // 15. загрузка справочника СвойствДокумента
        private bool isLoadOperationState;
        public bool IsLoadOperationState
        {
            get { return isLoadOperationState; }
            set { isLoadOperationState = value; }
        }

        // 30. Загрузка документа Требование на разгрузку
        private bool isLoadDocTrebovanie;
        public bool IsLoadDocTrebovanie
        {
            get { return isLoadDocTrebovanie; }
            set { isLoadDocTrebovanie = value; }
        }

        // 31. Загрузка документа Инвентаризация
        private bool isLoadDocInventory;
        public bool IsLoadDocInventory
        {
            get { return isLoadDocInventory; }
            set { isLoadDocInventory = value; }
        }
        // 32. Загрузка документа Заявка покупателя
        private bool isLoadDocInvoiceOrder;
        public bool IsLoadDocInvoiceOrder
        {
            get { return isLoadDocInvoiceOrder; }
            set { isLoadDocInvoiceOrder = value; }
        }
        
        // 33. Загрузка документа Спецификация перемещения
        private bool isLoadDocSpecPeremesh;
        public bool IsLoadDocSpecPeremesh
        {
            get { return isLoadDocSpecPeremesh; }
            set { isLoadDocSpecPeremesh = value; }
        }
        
        // 34. Загрузка документа Спецификация прихода
        private bool isLoadDocSpecPrihoda;
        public bool IsLoadDocSpecPrihoda
        {
            get { return isLoadDocSpecPrihoda; }
            set { isLoadDocSpecPrihoda = value; }
        }
        
        // 35. Загрузка документа Спецификация расхода
        private bool isLoadDocSpecRashoda;
        public bool IsLoadDocSpecRashoda
        {
            get { return isLoadDocSpecRashoda; }
            set { isLoadDocSpecRashoda = value; }
        }

 
        // 40. окно сообщений
        private bool isShowMessageInLogWindow;
        public bool IsShowMessageInLogWindow
        {
            get { return isShowMessageInLogWindow; }
            set { isShowMessageInLogWindow = value; }
        }

        private bool isRunOutBound;
        
        [DevExpress.Xpo.DisplayName(@"Включить выгрузку из NET в 7.7")]
        public bool IsRunOutBound
        {
            get { return isRunOutBound; }
            set { isRunOutBound = value; }
        }

        private bool isRunInBound;
        [DevExpress.Xpo.DisplayName(@"Включить загрузку из 7.7 в NET")]
        public bool IsRunInBound
        {
            get { return isRunInBound; }
            set { isRunInBound = value; }
        }

        private bool isTimerEnabled;
        [DevExpress.Xpo.DisplayName(@"Включить таймер")]
        public bool IsTimerEnabled
        {
            get { return isTimerEnabled; }
            set { isTimerEnabled = value; }
        }

        // 52. флаг отмены загрузки
        private bool isCancelStatus;
        public bool IsCancelStatus
        {
            get { return isCancelStatus; }
            set { isCancelStatus = value; }
        }

        

      #endregion      
        
        private List<LogMessages> listOfMessages;
        [XafDisplayName("Логи загрузки")]
        public List<LogMessages> ListOfMessages { get { return listOfMessages; } }

        private List<LogMessages> listOfOUTMessages;
        [XafDisplayName("Логи выгрузки")]
        public List<LogMessages> ListOfOUTMessages { get { return listOfOUTMessages; } }

        private BindingList<LogTypeMessages> listOfTypeMessages;
        [XafDisplayName("Список загружаемых объектов")]
        public BindingList<LogTypeMessages> ListOfTypeMessages { get { return listOfTypeMessages; } }

        private BindingList<LogTypeMessages> listOfTypeOUTMessages;
        [XafDisplayName("Список выгружаемых объектов")]
        public BindingList<LogTypeMessages> ListOfTypeOUTMessages { get { return listOfTypeOUTMessages; } }

        // конструктор по умолчанию
        public SQL_Exchange_Inbound()
        {
            IsLoadStockRooms = true;
            isLoadGoods = true;
            IsLoadUnitOfGoods = true;
            IsLoadStorageCodes = true;
            IsLoadProperties = true;
            IsLoadCountries = true;
            IsLoadManufacturers = true;
            IsLoadOKEI = true;
            IsLoadClients = true;
            IsLoadJobTypes = true;
            IsLoadBarcodesOfGoods = true;
            IsLoadLogistics = true;
            IsLoadCustomerDivisions = true;
            IsLoadCustomerStores = true;
            IsLoadWarehouses = true;
            IsLoadDocTrebovanie = true;
            IsLoadDocInventory = true;
            IsLoadDocInvoiceOrder = true;
            IsLoadDocSpecPeremesh = true;
            IsLoadDocSpecRashoda = true;
            IsLoadDocSpecPrihoda = true;
            IsShowMessageInLogWindow = true;
            IsRunOutBound = true;
            IsLoadDocProperties = true;
            IsLoadUsers = true;
            IsLoadDocJobeProperties = true;
            IsLoadOperationState = true;
            listOfMessages = new List<LogMessages>();
            listOfTypeMessages = new BindingList<LogTypeMessages>();

            listOfOUTMessages = new List<LogMessages>();
            listOfTypeOUTMessages = new BindingList<LogTypeMessages>();

            // заполнение списка видов загружаемых объектов:
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_BarcodesOfGoods) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Countries) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Clients) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_CustomerStores) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_CustomerUnits) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocProperties) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocJobeProperties) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_JobTypes) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Goods) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Logistics) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Manufacturers) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_OKEI) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_OperationState) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Properties) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_StockRooms) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_StorageCodes) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Units) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Users) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_Warehouses) });            
            
            
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocTrebovanieHeaders) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocInventoryHeaders) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocInvoiceOrderHeaders) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocSpecPeremeshHeaders) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocSpecRashodaHeaders) });
            ListOfTypeMessages.Add(new LogTypeMessages() { Id = ListOfTypeMessages.Count + 1, objType = typeof(SUTZ_NET_DocSpecPrihodaHeaders) });

            // заполнение списка выгружаемых объектов:
            ListOfTypeOUTMessages.Add(new LogTypeMessages() { Id = ListOfTypeOUTMessages.Count + 1, objType = typeof(NET_SUTZ_DocJobeProperties) });
            ListOfTypeOUTMessages.Add(new LogTypeMessages() { Id = ListOfTypeOUTMessages.Count + 1, objType = typeof(NET_SUTZ_MobileWorkUnit) });
            ListOfTypeOUTMessages.Add(new LogTypeMessages() { Id = ListOfTypeOUTMessages.Count + 1, objType = typeof(NET_SUTZ_DocSpecPrihodaHeaders) });
            ListOfTypeOUTMessages.Add(new LogTypeMessages() { Id = ListOfTypeOUTMessages.Count + 1, objType = typeof(NET_SUTZ_DocSpecPeremeshHeaders) });
            ListOfTypeOUTMessages.Add(new LogTypeMessages() { Id = ListOfTypeOUTMessages.Count + 1, objType = typeof(NET_SUTZ_DocSpecRashHeaders) });
        }

        public SQL_Exchange_Inbound(IObjectSpace objSpace, CancellationToken token_) : this()
        {
            //delegateForShowMessage = sender;
            objectSpace = objSpace;
            clToken = token_;
            //delegateForCalculateItems = delegate2_;            
        }

        //public bool inboundAllDataByMultiThread()
        //{
        //    int procCount = Environment.ProcessorCount;
        //    LimitedConcurrencyLevelTaskScheduler lctsParent = new LimitedConcurrencyLevelTaskScheduler(procCount - 1);

        //    TaskFactory factoryParent = new TaskFactory(lctsParent);
        //    List<Task> taskList = new List<Task>();

        //    // 2. Загрузка справочников.
        //    if (IsLoadWarehouses)   
        //    {
        //        taskList.Add(factoryParent.StartNew(() => this.LoadAllWarehouses(clToken), clToken));
        //    }     // 2.6 загрузка справочника складов

        //    if (IsLoadManufacturers)
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllManufacturers()));
        //    }

        //    if (IsLoadCountries)    
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllCountries()));
        //    }

        //    if (IsLoadUsers)        
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllUsers()));
        //    }

        //    if (IsLoadStockRooms)   
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllStockrooms()));
        //    }
            
        //    if (IsLoadClients)      
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllClients()));
        //    }    // 2.2 загрузка клиентов
            
        //    if (IsLoadCustomerStores) 
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllCustomerStories()));
        //    }     // 2.6 загрузка справочника складов клиентов

        //    if (IsLoadCustomerDivisions) 
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllCustomerDivisions()));
        //    }     // 2.6 загрузка справочника подразделений 

        //    // 2.4 загрузка справочника номенклатуры и всех связанных справочников:            
        //    if (IsLoadOKEI)         
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllOKEI()));
        //    }     // 2.6 загрузка справочника классификатора единиц измерения
            
        //    if (isLoadGoods)        
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllGoods()));
        //    }

        //    if (IsLoadUnitOfGoods)  
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllUnits()));
        //    }

        //    if (IsLoadProperties)   
        //    {
        //        taskList.Add(factoryParent.StartNew(() => this.LoadAllProperties(clToken),clToken));
        //    }

        //    if (IsLoadDocJobeProperties) 
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocJobeProperties()));
        //    }

        //    if (IsLoadBarcodesOfGoods) 
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllBarcodeOfGoods()));                
        //    }

        //    // загрузка всех других справочников
        //    if (IsLoadJobTypes)     
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAlljobTypes()));
        //    }   // 2.5 загрузка справочника ВидыРабот

        //    if (IsLoadLogistics)    
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadLogistics()));
        //    }     // 2.6 загрузка справочника Логистика

        //    if (IsLoadStorageCodes) 
        //    { 
        //        taskList.Add(factoryParent.StartNew(() => LoadAllStorageCodes()));
        //    }// 2.3 загрузка кодов размещения
            
        //    // 3. загрузка документов
        //    if (IsLoadDocTrebovanie)    
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocsTrebovanie()));
        //    }  // 3.1 Требование на разгрузку

        //    if (IsLoadDocInventory)     
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocsInventory()));
        //    }   // 3.2 Инвентаризация
            
        //    if (IsLoadDocInvoiceOrder)  
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocsInvoiceOrder()));
        //    }// 3.3 Заявка покупателя

        //    if (IsLoadDocSpecPrihoda)   
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocsSpecPrihoda()));
        //    } // 3.4 Спецификация прихода

        //    if (IsLoadDocSpecPeremesh)  
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocsSpecPeremesh()));
        //    }// 3.5 Спецификация перемещения

        //    if (IsLoadDocSpecRashoda)   
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocsSpecRashoda()));
        //    } // 3.6 Спецификация расхода

        //    // 4. загрузка свойств документов:
        //    if (IsLoadDocProperties) 
        //    {
        //        taskList.Add(factoryParent.StartNew(() => LoadAllDocProperties()));
        //    } // 4.1 Справочник СвойстваДокументов
            
        //    return true;
        //}

        // метод выполняет загрузку всех данных из СУТЗ 1С:7.7 в синхронном однопоточном режиме

        public bool InboundAllData()
        {
            bool isLoading = true;

            // 2. Загрузка справочников.
            if (IsLoadWarehouses)   {isLoading = LoadAllWarehouses(); }     // 2.6 загрузка справочника складов
            if (IsLoadManufacturers){isLoading = LoadAllManufacturers();}
            if (IsLoadCountries)    {isLoading = LoadAllCountries();}
            if (IsLoadUsers)        {isLoading = LoadAllUsers(); }

            if (IsLoadStockRooms)   {isLoading = LoadAllStockrooms();}
            
            if (IsLoadClients)      {isLoading = LoadAllClients(); }    // 2.2 загрузка клиентов
            if (IsLoadCustomerStores) {isLoading = LoadAllCustomerStories(); }     // 2.6 загрузка справочника складов клиентов
            if (IsLoadCustomerDivisions) {isLoading = LoadAllCustomerDivisions(); }     // 2.6 загрузка справочника подразделений 
            // 2.4 загрузка справочника номенклатуры и всех связанных справочников:
            
            if (IsLoadOKEI)         {isLoading = LoadAllOKEI(); }     // 2.6 загрузка справочника классификатора единиц измерения
            if (isLoadGoods)        {isLoading = LoadAllGoods();}
            if (IsLoadUnitOfGoods)  {isLoading = LoadAllUnits();}
            if (IsLoadProperties)   {isLoading = LoadAllProperties();}
            
            if (IsLoadBarcodesOfGoods) {isLoading = LoadAllBarcodeOfGoods(); }

            // загрузка всех других справочников
            if (IsLoadJobTypes)     {isLoading = LoadAlljobTypes(); }   // 2.5 загрузка справочника ВидыРабот
            if (IsLoadLogistics)    {isLoading = LoadLogistics(); }     // 2.6 загрузка справочника Логистика
            if (IsLoadStorageCodes) { isLoading = LoadAllStorageCodes(); }// 2.3 загрузка кодов размещения
            if (IsLoadOperationState) { isLoading = LoadAllOperationStates(); }// 2.3 загрузка кодов размещения
            
            // 3. загрузка документов
            if (IsLoadDocTrebovanie)    {isLoading = LoadAllDocsTrebovanie();}  // 3.1 Требование на разгрузку
            if (IsLoadDocInventory)     {isLoading = LoadAllDocsInventory();}   // 3.2 Инвентаризация
            if (IsLoadDocInvoiceOrder)  {isLoading = LoadAllDocsInvoiceOrder();}// 3.3 Заявка покупателя
            if (IsLoadDocSpecPrihoda)   {isLoading = LoadAllDocsSpecPrihoda();} // 3.4 Спецификация прихода
            if (IsLoadDocSpecPeremesh)  {isLoading = LoadAllDocsSpecPeremesh();}// 3.5 Спецификация перемещения
            if (IsLoadDocSpecRashoda)   {isLoading = LoadAllDocsSpecRashoda();} // 3.6 Спецификация расхода

            // 4. загрузка свойств документов:
            if (IsLoadDocProperties) { isLoading = LoadAllDocProperties(); } // 4.1 Справочник СвойстваДокументов
            if (IsLoadDocJobeProperties) { isLoading = LoadAllDocJobeProperties(); }// 4.2 пара документ-вид работы, загружается после документов.

            // 4. вызов выгрузки из NET в 7.7:
            //if (IsRunOutBound) { isLoading = runOutBoundData(); }

            // зафиксировать изменения по ранее записанным объектам
            //if (((ICollection)objectSpace.ModifiedObjects).Count>0)
            //{
            //    objectSpace.CommitChanges();
            //}
            return isLoading;
        }

        // вызов выгрузки данных в 7.7
        private bool runOutBoundData()
        {
            throw new NotImplementedException();
        }

        private bool runCommitChanges(Type commonType, string strMessage)
        {
            //if (((ICollection)objectSpace.ModifiedObjects).Count > 0)
            //{
                try
                {
                    objectSpace.CommitChanges();
                }
                catch (System.Exception ex)
                {
                    lokSendAndShowMessage(String.Format("Ошибка при вызове CommitChanges() ex={0}", ex.Message), "Exception", ex);
                    IList objList = objectSpace.ModifiedObjects;
                    foreach (Object item in objectSpace.ModifiedObjects)
	                {
                        objectSpace.RemoveFromModifiedObjects(item);
	                }                    
                    return false;
                }
           // }
            return true;
        }

        private T findObjectByGUID<T>(Guid param)
        {
            T findedElement = objectSpace.FindObject<T>(new BinaryOperator("idGUID", param, BinaryOperatorType.Equal),
true);
            return findedElement;
        }

        // метод определяет и создает разделитель, если такого разделителя еще нет в справочнике:
        private Delimeters findDelimiterByGUID(Guid guidParam)
        {
            Guard.ArgumentNotNull(guidParam, "guidParam");

            Delimeters findedElement = objectSpace.FindObject<Delimeters>(new BinaryOperator("DelimeterID", guidParam, BinaryOperatorType.Equal), true);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Delimeters>();
                findedElement.DefaultDelimeter = false;
                findedElement.DelimeterID = guidParam;
                findedElement.Description = "Разделитель учета. Автоматически создан автоматом обмена" + DateTime.Now.ToLocalTime().ToLongTimeString();
                try
                {
                    findedElement.Save();
                    //objectSpace.CommitChanges();
                    logger.Trace("Записан новый разделитель учета, guidParam = {0}", guidParam);
                }
                catch (System.Exception ex)
                {
                    logger.Trace(ex,"Ошибка записи нового элемента справочника разделителя учета, guidParam = {0}",guidParam);
                    return null;
                }
            }
            return findedElement;
        }

        // метод выводит сообщения в форму и в логи.
        // Передаваемые параметры: "strMessage" - сообщение;
        //                          traceLevel - строка, уровень сообщения.
        private void lokSendAndShowMessage(String strMessage, string traceLevel, Exception ex)
        {
            //listOfMessages.Add(new LogMessages() { Id = ((listOfMessages.Count) + 1), MessageText = strMessage, MessageTime = DateTime.Now, MessageType = "info" });
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
            else if (traceLevel.Equals("Error"))
            {
                logger.Warn(strMessage);
            }
            else if (traceLevel.Equals("Exception"))
            {
                logger.Error(ex, "Exception при вызове CommitChanges()!");
            }
        }

        // метод передает сигнал на изменение счетчика загруженных объектов опрделенного типа:
        private void lokSendMessageForObjectType(Type objType, int rowCounts)
        {
            delegateForCalculateItems(objType, rowCounts);
        }

        #region Загрузчик Отборочных листов
        // загрузчик документа Спецификация расхода
        private DocSpecRashoda loadOneDocSpecRashoda(SUTZ_NET_DocSpecRashodaHeaders record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            DocSpecRashoda findedElement = findObjectByGUID<DocSpecRashoda>(record.iddSUTZ_guid);            
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocSpecRashoda>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            
            findedElement.DocDateTime   = record.DocDateTime;
            findedElement.DocState      = (DocStates)record.StatusDoc;
            findedElement.Comment       = record.Comment;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 1.3 установка документа-основания:
            if ((record.DocBaseID != Guid.Empty) && (record.DocBaseID != null))
            {
                findedElement.DocBase = findObjectByGUID<BaseDocument>(record.DocBaseID);
                if (findedElement.DocBase == null)
                {
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Отборочный лист {0}, row_id={1}. Не найден документ-основание по guid={2}", record.DocNo, record.row_id, record.DocBaseID), "Error", null);
                }
            }

            findedElement.DocNo = record.DocNo;
            if ((record.StockRoomFrom != Guid.Empty) && (record.StockRoomFrom != null))
            {
                findedElement.StockRoomFrom = findObjectByGUID<StockRooms>(record.StockRoomFrom);
                if (findedElement.StockRoomFrom == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Отборочный лист {0}, row_id={1}. Не найдена Кладовая-отправитель по guid={2}", record.DocNo, record.row_id, record.StockRoomFrom), "Error", null);
                }
            }

            findedElement.OrderClosed   = record.OrderClosed;
            findedElement.TovarOtobran  = record.TovarOtobran;
            findedElement.TovarPoluchen = record.TovarPoluchen;

            // 3. загрузка строк табличной части документа
            foreach (SUTZ_NET_DocSpecRashodaGoods item in record.SUTZ_NET_DocSpecRashodaGoods)
            {
                var docRecordQuery = from c in findedElement.DocListOfGoods orderby c.LineNo where c.DocLineUID == item.DocLineUID select c;
                DocSpecRashodaGoods docRecord = null;
                if (docRecordQuery.Count()==0)
                {
                    docRecord = objectSpace.CreateObject<DocSpecRashodaGoods>();
                    docRecord.DocLineUID = item.DocLineUID;
                }
                else
                {
                    docRecord = docRecordQuery.First();
                }
                docRecord.LineNo    = item.DocLineNo;
                docRecord.Delimeter = findedElement.Delimeter;

                // 3.2 поиск товара книжного
                if ((item.GoodID != Guid.Empty) && (item.GoodID != null))
                {
                    docRecord.Good = findObjectByGUID<Goods>(item.GoodID);
                    if (docRecord.Good == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Отборочный лист {0}, row_id={1}. Не найден товар по guid={2}", record.DocNo, item.row_id, item.GoodID), "Error", null);
                        return null;
                    }
                }

                // 3.3 поиск единицы измерения
                if ((item.UnitID != Guid.Empty) && (item.UnitID != null))
                {
                    docRecord.Unit = findObjectByGUID<Units>(item.UnitID);
                    if (docRecord.Unit == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Отборочный лист {0}, row_id={1}. Не найдена единица измерения по guid={2}", record.DocNo, item.row_id, item.UnitID), "Error", null);
                        return null;
                    }
                }

                if (docRecord.Unit != null)
                {
                    docRecord.Coeff = (decimal)docRecord.Unit.Koeff;
                }
                docRecord.QuantityOfUnits = item.QuantityOfUnits;
                docRecord.QuantityOfItems = item.QuantityOfItems;
                docRecord.TotalQuantity = item.TotalQuantity;
                docRecord.QuantityFact = item.QuantityFact;
                docRecord.QuantityOrig = item.QuantityOrig;

                docRecord.BestBefore = item.BestBefore;
                // Характеристика номенклатуры
                if ((item.PropertyID != Guid.Empty) && (item.PropertyID != null))
                {
                    docRecord.Property = findObjectByGUID<PropertiesOfGoods>(item.PropertyID);
                    if (docRecord.Property == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Отборочный лист {0}, row_id={1}. Не найдена Характеристика товара по guid={2}", record.DocNo, item.row_id, item.PropertyID), "Error", null);
                        return null;
                    }
                }

                // Взять из
                if ((item.TakeFrom != Guid.Empty) && (item.TakeFrom != null))
                {
                    docRecord.TakeFrom = findObjectByGUID<StorageCodes>(item.TakeFrom);
                    if (docRecord.TakeFrom == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Отборочный лист {0}, row_id={1}. Не найден код размещения ВзятьИз по guid={2}", record.DocNo, item.row_id, item.TakeFrom), "Error", null);
                        return null;
                    }
                }
                findedElement.DocListOfGoods.Add(docRecord);
            }

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }
        // Загрузчик всех Спецификаций Расхода
        private bool LoadAllDocsSpecRashoda()
        {
            XPQuery<SUTZ_NET_DocSpecRashodaHeaders> records = new XPQuery<SUTZ_NET_DocSpecRashodaHeaders>(objectSpace.Session());
            var listOfDocs = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocSpecRashodaHeaders document in listOfDocs)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки спецификаций расхода товара по маркеру отмены!"), "Warning", null);
                    break;
                }
                DocSpecRashoda findedElement = loadOneDocSpecRashoda(document);
                bool isCommited = runCommitChanges( typeof(DocSpecRashoda), "Спецификация расхода");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен документ: {0}", findedElement), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_DocSpecRashodaHeaders), 1);
                    }
                }
            }
            return true;
        }
      #endregion

        #region Загрузчик Спецификация перемещения
        // Загрузчик документа Спецификация перемещения
        private DocSpecPeremeshenie loadOneDocSpecPeremesh(SUTZ_NET_DocSpecPeremeshHeaders record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            DocSpecPeremeshenie findedElement = findObjectByGUID<DocSpecPeremeshenie>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocSpecPeremeshenie>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 1.2 установка разделителя учета
            findedElement.Delimeter     = findDelimiterByGUID(record.Delimeter);
            findedElement.DocDateTime   = record.DocDateTime;
            findedElement.DocState      = (DocStates)record.StatusDoc;
            findedElement.Comment       = record.Comment;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 2.3 установка документа-основания:
            if ((record.DocBaseID != Guid.Empty) && (record.DocBaseID != null))
            {
                findedElement.DocBase = findObjectByGUID<BaseDocument>(record.DocBaseID);
                if (findedElement.DocBase == null)
                {
                    //hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Спецификация перемещения {0}, row_id={1}. Не найден документ-основание по guid={2}", record.DocNo, record.row_id, record.DocBaseID), "Error", null);
                }
            }

            findedElement.DocNo = record.DocNo;
            // кладовая-получатель
            if ((record.StockRoomFrom != Guid.Empty) && (record.StockRoomFrom != null))
            {
                findedElement.StockRoomFrom = findObjectByGUID<StockRooms>(record.StockRoomFrom);
                if (findedElement.StockRoomFrom == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Спецификация перемещения {0}, row_id={1}. Не найдена Кладовая-отправитель по guid={2}", record.DocNo, record.row_id, record.StockRoomFrom), "Error", null);
                }
            }

            // кладовая-получатель
            if ((record.StockRoomTo != Guid.Empty) && (record.StockRoomTo != null))
            {
                findedElement.StockRoomTo = findObjectByGUID<StockRooms>(record.StockRoomTo);
                if (findedElement.StockRoomTo == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Спецификация перемещения {0}, row_id={1}. Не найдена Кладовая-получатель по guid={2}", record.DocNo, record.row_id, record.StockRoomTo), "Error", null);
                }
            }
            findedElement.AllowMovement = record.AllowMovement;

            // 3. загрузка строк табличной части документа
            foreach (SUTZ_NET_DocSpecPeremeshGoods item in record.SUTZ_NET_DocSpecPeremeshGoods)
            {
                var docRecordQuery = from c in findedElement.DocListOfGoods orderby c.LineNo where c.DocLineUID == item.DocLineUID select c;
                DocSpecPeremeshGoods docRecord = null;
                if (docRecordQuery.Count() == 0)
                {
                    docRecord = objectSpace.CreateObject<DocSpecPeremeshGoods>();
                    docRecord.DocLineUID = item.DocLineUID;
                }
                else
                {
                    docRecord = docRecordQuery.First();
                }

                docRecord.LineNo = item.DocLineNo;
                docRecord.Delimeter = findedElement.Delimeter;

                // 3.2 поиск товара книжного и фактического
              if ((item.GoodID != Guid.Empty) && (item.GoodID != null))
                {
                    docRecord.Good = findObjectByGUID<Goods>(item.GoodID);
                    if (docRecord.Good == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация перемещения {0}, row_id={1}. Не найден товар по guid={2}", record.DocNo, item.row_id, item.GoodID), "Error", null);
                        return null;
                    }
                }

                // 3.3 поиск единицы измерения
                if ((item.UnitID != Guid.Empty) && (item.UnitID != null))
                {
                    docRecord.Unit = findObjectByGUID<Units>(item.UnitID);
                    if (docRecord.Unit == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация перемещения {0}, row_id={1}. Не найдена единица измерения по guid={2}", record.DocNo, item.row_id, item.UnitID), "Error", null);
                        return null;
                    }
                }

                if (docRecord.Unit != null)
                {
                    docRecord.Coeff = (decimal)docRecord.Unit.Koeff;
                }
                docRecord.QuantityOfUnits = item.QuantityOfUnits;
                docRecord.QuantityOfItems = item.QuantityOfItems;
                docRecord.TotalQuantity = item.TotalQuantity;
                docRecord.BestBefore = item.BestBefore;
                docRecord.QuantityOrig = item.QuantityOrig;
                docRecord.QuantityFact = item.QuantityFact;

                // Характеристика номенклатуры
                if ((item.PropertyID != Guid.Empty) && (item.PropertyID != null))
                {
                    docRecord.Property = findObjectByGUID<PropertiesOfGoods>(item.PropertyID);
                    if (docRecord.Property == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация перемещения {0}, row_id={1}. Не найдена Характеристика товара по guid={2}", record.DocNo, item.row_id, item.PropertyID), "Error", null);
                        return null;
                    }
                }

                // Взять из
                if ((item.TakeFrom != Guid.Empty) && (item.TakeFrom != null))
                {
                    docRecord.TakeFrom = findObjectByGUID<StorageCodes>(item.TakeFrom);
                    if (docRecord.TakeFrom == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация перемещения {0}, row_id={1}. Не найден код размещения ВзятьИз по guid={2}", record.DocNo, item.row_id, item.TakeFrom), "Error", null);
                        return null;
                    }
                }

                // ПоложитьВ
                if ((item.PutTo != Guid.Empty) && (item.PutTo != null))
                {
                    docRecord.PutTo = findObjectByGUID<StorageCodes>(item.PutTo);
                    if (docRecord.PutTo == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация перемещения {0}, row_id={1}. Не найден код размещения ПоложитьВ по guid={2}", record.DocNo, item.row_id, item.PutTo), "Error", null);
                        return null;
                    }
                }

                // Точка передачи
                if ((item.ExchangePointID != Guid.Empty) && (item.ExchangePointID != null))
                {
                    docRecord.ExchangePoint = findObjectByGUID<StorageCodes>(item.ExchangePointID);
                    if (docRecord.ExchangePoint == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация перемещения {0}, row_id={1}. Не найден код размещения ТочкаПередачи по guid={2}", record.DocNo, item.row_id, item.ExchangePointID), "Error", null);
                        return null;
                    }
                }
                docRecord.QuantityOrig = item.QuantityOrig;
                docRecord.QuantityFact = item.QuantityFact;

                findedElement.DocListOfGoods.Add(docRecord);
            }
            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }
            
            record.Save();
            return findedElement;
        }
        // Загрузчик всех документов Спецификация перемещения
        private bool LoadAllDocsSpecPeremesh()
        {
            XPQuery<SUTZ_NET_DocSpecPeremeshHeaders> records = new XPQuery<SUTZ_NET_DocSpecPeremeshHeaders>(objectSpace.Session());
            var listOfDocs = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocSpecPeremeshHeaders document in listOfDocs)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Спецификаций перемещения товара по маркеру отмены!"), "Warning", null);
                    break;
                }
                DocSpecPeremeshenie findedElement = loadOneDocSpecPeremesh(document);
                bool isCommited = runCommitChanges(typeof(DocSpecPeremeshenie), "Спецификация перемещения");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен документ: {0}", findedElement), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_DocSpecPeremeshHeaders), 1);
                    }
                }
            }
            return true;
        }
        #endregion
     
        #region Загрузчик Инвентаризаций
       // Загрузчик документа Пересчет склада
        private DocInventory loadOneDocInventory(SUTZ_NET_DocInventoryHeaders record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            DocInventory findedElement = findObjectByGUID<DocInventory>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocInventory>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            findedElement.DocDateTime = record.DocDateTime;
            findedElement.DocState = (DocStates)record.StatusDoc;
            findedElement.Comment = record.Comment;
            findedElement.DocNo = record.DocNo;
            findedElement.TypeDoc = (enDocStatesInventory)record.TypeDoc;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 2.3 установка документа-основания:
            if ((record.DocBaseID != Guid.Empty) && (record.DocBaseID != null))
            {
                findedElement.DocBase = findObjectByGUID<BaseDocument>(record.DocBaseID);
                if (findedElement.DocBase == null)
                {
                    //hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Инвентаризация {0}, row_id={1}. Не найден документ-основание по guid={2}", record.DocNo, record.row_id, record.DocBaseID), "Error", null);
                }
            }
            
            // кладовая-получатель
            if ((record.StockRoomTo != Guid.Empty) && (record.StockRoomTo != null))
            {
                findedElement.StockRoomTo = findObjectByGUID<StockRooms>(record.StockRoomTo);
                if (findedElement.StockRoomTo == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Спецификация перемещения {0}, row_id={1}. Не найдена Кладовая-получатель по guid={2}", record.DocNo, record.row_id, record.StockRoomTo), "Error", null);
                }
            }            

            // 3. загрузка строк табличной части документа
            foreach (SUTZ_NET_DocInventoryGoods item in record.SUTZ_NET_DocInventoryGoods)
            {
                var docRecordQuery = from c in findedElement.DocListOfGoods orderby c.LineNo where c.DocLineUID == item.DocLineUID select c;
                DocInventoryGoods docRecord = null;
                if (docRecordQuery.Count() == 0)
                {
                    docRecord = objectSpace.CreateObject<DocInventoryGoods>();
                    docRecord.DocLineUID = item.DocLineUID;
                }
                else
                {
                    docRecord = docRecordQuery.First();
                }
                docRecord.LineNo = item.DocLineNo;
                docRecord.Delimeter = findedElement.Delimeter;

                // 3.2 поиск товара книжного и фактического
                if ((item.GoodID != Guid.Empty) && (item.GoodID != null))
                {
                    docRecord.Good = findObjectByGUID<Goods>(item.GoodID);
                    if (docRecord.Good == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найден книжный товар по guid={2}", record.DocNo, item.row_id, item.GoodID), "Error", null);
                        return null;
                    }
                }

                if ((item.GoodIDFact != Guid.Empty) && (item.GoodIDFact != null))
                {
                    docRecord.GoodFact = findObjectByGUID<Goods>(item.GoodIDFact);
                    if (docRecord.GoodFact == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найден фактический товар по guid={2}", record.DocNo, item.row_id, item.GoodID), "Error", null);
                        return null;
                    }
                }

                // 3.3 поиск единицы измерения
                if ((item.UnitID != Guid.Empty) && (item.UnitID != null))
                {
                    docRecord.Unit = findObjectByGUID<Units>(item.UnitID);
                    if (docRecord.Unit == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найдена единица измерения по guid={2}", record.DocNo, item.row_id, item.UnitID), "Error", null);
                        return null;
                    }
                }
                
                if ((item.UnitIDFact != Guid.Empty) && (item.UnitIDFact != null))
                {
                    docRecord.UnitFact = findObjectByGUID<Units>(item.UnitIDFact);
                    if (docRecord.UnitFact == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найдена факт.единица измерения по guid={2}", record.DocNo, item.row_id, item.UnitIDFact), "Error", null);
                        return null;
                    }
                }

                if (docRecord.Unit != null)
                {
                    docRecord.Coeff = (decimal)docRecord.Unit.Koeff;
                }
                if (docRecord.UnitFact != null)
                {
                    docRecord.CoeffFact = (decimal)docRecord.UnitFact.Koeff;
                }

                docRecord.QuantityOfUnits   = item.QuantityOfUnits;
                docRecord.QuantityOfItems   = item.QuantityOfItems;
                docRecord.TotalQuantity     = item.TotalQuantity;

                docRecord.QuantityOfUnitsFact = item.QuantityOfUnitsFact;
                docRecord.QuantityOfItemsFact = item.QuantityOfItemsFact;
                docRecord.TotalQuantityFact   = item.TotalQuantityFact;

                docRecord.BestBefore = item.BestBefore;
                docRecord.BestBeforeFact = item.BestBeforeFact;

                if ((item.TakeFrom != Guid.Empty) && (item.TakeFrom != null))
                {
                    docRecord.TakeFrom = findObjectByGUID<StorageCodes>(item.TakeFrom);
                    if (docRecord.TakeFrom == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найден код размещения ВзятьИз по guid={2}", record.DocNo, item.row_id, item.TakeFrom), "Error", null);
                        return null;
                    }
                }

                if ((item.PropertyID != Guid.Empty) && (item.PropertyID != null))
                {
                    docRecord.Property = findObjectByGUID<PropertiesOfGoods>(item.PropertyID);
                    if (docRecord.Property == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найдена Характеристика товара по guid={2}", record.DocNo, item.row_id, item.PropertyID), "Error", null);
                        return null;
                    }
                }

                if ((item.PropertyIDFact != Guid.Empty) && (item.PropertyIDFact != null))
                {
                    docRecord.PropertyFact = findObjectByGUID<PropertiesOfGoods>(item.PropertyIDFact);
                    if (docRecord.PropertyFact == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Инвентаризация {0}, row_id={1}. Не найдена факт. Характеристика товара по guid={2}", record.DocNo, item.row_id, item.PropertyIDFact), "Error", null);
                        return null;
                    }
                }
                findedElement.DocListOfGoods.Add(docRecord);
            }

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }

        // Загрузчик всех документов Пересчет склада
        private bool LoadAllDocsInventory()
        {
            XPQuery<SUTZ_NET_DocInventoryHeaders> records = new XPQuery<SUTZ_NET_DocInventoryHeaders>(objectSpace.Session());
            var listOfDocs = from c in records where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocInventoryHeaders document in listOfDocs)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Инвентаризаций по маркеру отмены!"), "Warning", null);
                    break;
                }
                DocInventory findedElement = loadOneDocInventory(document);
                bool isCommited = runCommitChanges(typeof(DocInventory), "Инвентаризация");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен документ: {0}", findedElement), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_DocInventoryHeaders), 1);
                    }
                }
            }
            return true;
        }
       #endregion

        #region Загрузчик Спецификаций прихода
        // Загрузчик документа Спецификация прихода
        private DocSpecPrihoda loadOneDocSpecPrihoda(SUTZ_NET_DocSpecPrihodaHeaders record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            DocSpecPrihoda findedElement = findObjectByGUID<DocSpecPrihoda>(record.iddSUTZ_guid);            
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocSpecPrihoda>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter); 
            findedElement.DocDateTime = record.DocDateTime;
            findedElement.DocState = (DocStates)record.StatusDoc;
            findedElement.Comment = record.Comment;
            findedElement.DocNo = record.DocNo;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 1.3 установка документа-основания:
            if ((record.DocBaseID != Guid.Empty) && (record.DocBaseID != null))
            {
                findedElement.DocBase = findObjectByGUID<BaseDocument>(record.DocBaseID);
                if (findedElement.DocBase == null)
                {
                    //hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Спецификация прихода {0}, row_id={1}. Не найден документ-основание по guid={2}", record.DocNo, record.row_id, record.DocBaseID), "Error", null);
                }
            }
            // кладовая-получатель
            if ((record.StockRoomTo != Guid.Empty) && (record.StockRoomTo != null))
            {
                findedElement.StockRoomTo = findObjectByGUID<StockRooms>(record.StockRoomTo);
                if (findedElement.StockRoomTo == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Спецификация прихода {0}, row_id={1}. Не найдена Кладовая-получатель по guid={2}", record.DocNo, record.row_id, record.StockRoomTo), "Error", null);
                }
            }
            
            findedElement.AllowSender = record.AllowCheckIn;
            findedElement.AllowMovement = record.AllowMovement;

            // 3. загрузка строк табличной части документа
            foreach (SUTZ_NET_DocSpecPrihodaGoods item in record.SUTZ_NET_DocSpecPrihodaGoods)
            {
                var docRecordQuery = from c in findedElement.DocListOfGoods orderby c.LineNo where c.DocLineUID == item.DocLineUID select c;
                DocSpecPrihodaGoods docRecord = null;
                if (docRecordQuery.Count() == 0)
                {
                    docRecord = objectSpace.CreateObject<DocSpecPrihodaGoods>();
                    docRecord.DocLineUID = item.DocLineUID;
                }
                else
                {
                    docRecord = docRecordQuery.First();
                }
                docRecord.LineNo = item.DocLineNo;
                docRecord.Delimeter = findedElement.Delimeter;

                // 3.2 поиск товара
                if ((item.GoodID != Guid.Empty) && (item.GoodID != null))
                {
                    docRecord.Good = findObjectByGUID<Goods>(item.GoodID);
                    if (docRecord.Good == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация прихода {0}, row_id={1}. Не найден товар по guid={2}", record.DocNo, item.row_id, item.GoodID), "Error", null);
                        return null;
                    }
                }

                // 3.3 поиск единицы измерения
                if ((item.UnitID != Guid.Empty) && (item.UnitID != null))
                {
                    docRecord.Unit = findObjectByGUID<Units>(item.UnitID);
                    if (docRecord.Unit == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация прихода {0}, row_id={1}. Не найдена единица измерения по guid={2}", record.DocNo, item.row_id, item.UnitID), "Error", null);
                        return null;
                    }
                }

                if (docRecord.Unit != null)
                {
                    docRecord.Coeff = (decimal)docRecord.Unit.Koeff;
                }
                docRecord.QuantityOfUnits = item.QuantityOfUnits;
                docRecord.QuantityOfItems = item.QuantityOfItems;
                docRecord.TotalQuantity   = item.TotalQuantity;
                docRecord.BestBefore      = item.BestBefore;
                docRecord.QuantityFact    = item.QuantityFact;
                docRecord.QuantityOrig    = item.QuantityOrig;

                // Характеристика номенклатуры
                if ((item.PropertyID != Guid.Empty) && (item.PropertyID != null))
                {
                    docRecord.Property = findObjectByGUID<PropertiesOfGoods>(item.PropertyID);
                    if (docRecord.Property == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация прихода {0}, row_id={1}. Не найдена Характеристика товара по guid={2}", record.DocNo, item.row_id, item.PropertyID), "Error", null);
                        return null;
                    }
                }

                // Взять из
                if ((item.TakeFrom != Guid.Empty) && (item.TakeFrom != null))
                {
                    docRecord.TakeFrom = findObjectByGUID<StorageCodes>(item.TakeFrom);
                    if (docRecord.TakeFrom == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация прихода {0}, row_id={1}. Не найден код размещения ВзятьИз по guid={2}", record.DocNo, item.row_id, item.TakeFrom), "Error", null);
                        return null;
                    }
                }

                // ПоложитьВ
                if ((item.PutTo != Guid.Empty) && (item.PutTo != null))
                {
                    docRecord.PutTo = findObjectByGUID<StorageCodes>(item.PutTo);
                    if (docRecord.PutTo == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация прихода {0}, row_id={1}. Не найден код размещения ПоложитьВ по guid={2}", record.DocNo, item.row_id, item.PutTo), "Error", null);
                        return null;
                    }
                }
                // PalletLabel
                if (item.Barcode.Length!=0)
                {
                    XPQuery<PalletLabels> qPalletLabels = new XPQuery<PalletLabels>(objectSpace.Session());

                    docRecord.PalletLabel = qPalletLabels.Where(c => c.Barcode == item.Barcode).FirstOrDefault();
                    if (docRecord.PalletLabel == null)
                    {
                        hasError += 1;
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Спецификация прихода {0}, row_id={1}. Не найдена паллетная этикетка по ШК={2}", record.DocNo, item.row_id, item.Barcode), "Error", null);
                        return null;
                    }
                }
                findedElement.DocListOfGoods.Add(docRecord);
            }

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }
        // Загрузчик всех документов Спецификация прихода
        private bool LoadAllDocsSpecPrihoda()
        {
            XPQuery<SUTZ_NET_DocSpecPrihodaHeaders> records = new XPQuery<SUTZ_NET_DocSpecPrihodaHeaders>(objectSpace.Session());
            var listOfDocs = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocSpecPrihodaHeaders document in listOfDocs)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Спецификаций прихода по маркеру отмены!"), "Warning", null);
                    break;
                }
                DocSpecPrihoda findedElement = loadOneDocSpecPrihoda(document);
                bool isCommited = runCommitChanges(typeof(DocSpecPrihoda), "Спецификация прихода");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен документ: {0}", findedElement), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_DocSpecPrihodaHeaders), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Заявок покупателя
        // Загрузчик одного документа Заявка покупателя
        private DocInvoiceOrder loadOneDocInvoiceOrder(SUTZ_NET_DocInvoiceOrderHeaders record)
        {
            int hasError = 0;

            // 1. поиск объекта по iddguid. 
            DocInvoiceOrder findedElement = findObjectByGUID<DocInvoiceOrder>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocInvoiceOrder>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.DocDateTime = record.DocDateTime;
            findedElement.DocState = (DocStates)record.StatusDoc;
            findedElement.Comment = record.Comment;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 2.3 установка документа-основания:
            if ((record.DocBaseID!=Guid.Empty)&&(record.DocBaseID!=null))
            {
                findedElement.DocBase = findObjectByGUID<BaseDocument>(record.DocBaseID);
                if (findedElement.DocBase == null)
                {
                lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Заявка покупателя {0}, row_id={1}. Не найден документ-основание по guid={2}", record.DocNo, record.row_id, record.DocBaseID), "Error", null);
                }                
            }
            findedElement.DocNo         = record.DocNo;
            findedElement.ExecutionDate = record.ExecutionDate;
            if ((record.ClientID != Guid.Empty) && (record.ClientID != null))
            {
                findedElement.Client = findObjectByGUID<Clients>(record.ClientID);
                if (findedElement.Client == null)
                {
                    //hasError +=  1;
                lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Заявка покупателя {0}, row_id={1}. Не найден клиент по guid={2}", record.DocNo, record.row_id, record.ClientID), "Error", null);
                }                
            }
            
            findedElement.StatusASU     = (DocStatesASUInvoice)record.StatusDocASU;
            findedElement.Unloaded      = (record.Unloaded == 0) ? false : true;
            findedElement.Printed       = record.Printed;
            
            if ((record.Warehouse != Guid.Empty) && (record.Warehouse != null))
            {
                findedElement.Warehouse = findObjectByGUID<Warehouses>(record.Warehouse);
                if (findedElement.Warehouse == null)
                {
                    hasError +=  1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Заявка покупателя {0}, row_id={1}. Не найден Склад по guid={2}", record.DocNo, record.row_id, record.Warehouse), "Error", null);
                }                
            }
            findedElement.Shipped       = record.docShipped;
            findedElement.DeliveryType  = (enDeliveryTypes)record.typeOfShipped;

            if ((record.divisionOfClient != Guid.Empty) && (record.divisionOfClient != null))
            {
                findedElement.ClientDivision = findObjectByGUID<ClientDivisions>(record.divisionOfClient);
                if (findedElement.ClientDivision == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Заявка покупателя {0}, row_id={1}. Не найден Подразделение клиента по guid={2}", record.DocNo, record.row_id, record.divisionOfClient), "Error", null);
                }                
            }

            if ((record.ShopOfClient != Guid.Empty) && (record.ShopOfClient != null))
            {
                findedElement.ClientShop = findObjectByGUID<ClientShops>(record.ShopOfClient);
                if (findedElement.ClientShop != null)
                {
                    hasError +=  1;
                lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Заявка покупателя {0}, row_id={1}. Не найден Магазин клиента по guid={2}", record.DocNo, record.row_id, record.ShopOfClient), "Error", null);
                }                
            }

            // 3. загрузка строк табличной части документа
            foreach (SUTZ_NET_DocInvoiceOrderGoods item in record.SUTZ_NET_DocInvoiceOrderGoods)
            {
                var docRecordQuery = from c in findedElement.DocListOfGoods where c.DocLineUID == item.DocLineUID select c;
                DocInvoiceOrderGoods docRecord = null;

                if (docRecordQuery.Count() == 0)
                {
                    docRecord = objectSpace.CreateObject<DocInvoiceOrderGoods>();
                    docRecord.Delimeter = findedElement.Delimeter;
                    docRecord.DocLineUID = item.DocLineUID;
                }
                else
                {
                    docRecord = docRecordQuery.First();
                }
                docRecord.LineNo = item.DocLineNo;               

                if ((item.GoodID!=Guid.Empty)&&(item.GoodID!=null))
                {
                    docRecord.Good = findObjectByGUID<Goods>(item.GoodID);
                    if (docRecord.Good == null)
                    {
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Заявка покупателя {0}, row_id={1}, не найден товар по guid={2}", findedElement.DocNo, record.row_id, item.GoodID),"Error",null);
                    }
                }                

                // 3.3 поиск единицы измерения
                if ((item.UnitID != Guid.Empty) && (item.UnitID != null))
                {
                    docRecord.Unit = findObjectByGUID<Units>(item.UnitID);
                    if (docRecord.Unit == null)
                    {
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Заявка покупателя {0}, row_id={1}, не найдена Единица измерения по guid={2}", findedElement.DocNo, record.row_id, item.UnitID), "Error", null);
                    }
                }  

                if (docRecord.Unit != null)
                {
                    docRecord.Coeff = (decimal)docRecord.Unit.Koeff;
                }
                docRecord.QuantityOfUnits = item.QuantityOfUnits;
                docRecord.QuantityOfItems = item.QuantityOfItems;
                docRecord.TotalQuantity = item.TotalQuantity;

                docRecord.BestBefore = item.BestBefore;
                if ((item.PropertyID != Guid.Empty) && (item.PropertyID != null))
                {
                    docRecord.Property = findObjectByGUID<PropertiesOfGoods>(item.PropertyID);
                    if (docRecord.Property == null)
                    {
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке ТЧ документа Заявка покупателя {0}, row_id={1}, не найдена Характеристика по guid={2}", findedElement.DocNo, record.row_id, item.PropertyID), "Error", null);
                    }
                } 
                findedElement.DocListOfGoods.Add(docRecord);
            }

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();         
            return findedElement;
        }

        // Загрузчик всех документов Заявка покупателя
        private bool LoadAllDocsInvoiceOrder()
        {
            XPQuery<SUTZ_NET_DocInvoiceOrderHeaders> records = new XPQuery<SUTZ_NET_DocInvoiceOrderHeaders>(objectSpace.Session());
            var listOfDocs = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocInvoiceOrderHeaders document in listOfDocs)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Заявок покупателя по маркеру отмены!"), "Warning", null);
                    break;
                }
                DocInvoiceOrder findedElement = loadOneDocInvoiceOrder(document);
                bool isCommited = runCommitChanges(typeof(DocInvoiceOrder), "Заявка покупателя");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен документ: {0}", findedElement), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_DocInvoiceOrderHeaders), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Требования на разгрузку
       private DocTrebovanie loadOneDocTrebovanie(SUTZ_NET_DocTrebovanieHeaders record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            DocTrebovanie findedElement = findObjectByGUID<DocTrebovanie>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocTrebovanie>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 1.2 установка разделителя учета
            Delimeters delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Delimeter = delimeter;
            findedElement.DocDateTime = record.DocDateTime;
            findedElement.DocState    = (DocStates)record.StatusDoc;
            findedElement.Comment = record.Comment;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 2.3 установка документа-основания:
            if ((record.DocBaseID!=Guid.Empty)&&(record.DocBaseID!=null))
            {
                findedElement.DocBase = findObjectByGUID<BaseDocument>(record.DocBaseID);
            }         
            findedElement.DocNo = record.DocNo;         

            // 3. загрузка строк табличной части документа
            foreach (SUTZ_NET_DocTrebovanieGoods item in record.SUTZ_NET_DocTrebovanieGoodsCollection)
            {
                var docRecordQuery = from c in findedElement.DocTrebovanieGoodsCollection where c.DocLineUID == item.DocLineUID select c;
                DocTrebovanieGoods docRecord = null;
                if (docRecordQuery.Count() == 0)
                {
                    docRecord = objectSpace.CreateObject<DocTrebovanieGoods>();
                    docRecord.DocLineUID = item.DocLineUID;
                }
                else
                {
                    docRecord = docRecordQuery.First();
                }                
                docRecord.LineNo = item.DocLineNo;
                docRecord.Delimeter = delimeter;
                docRecord.Good = findObjectByGUID<Goods>(item.GoodID);
                if (docRecord.Good==null)
                {
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Требование на разгрузку {0}: не найден товар табличной части по guid={1}",record.DocNo,item.GoodID), "Error", null);
                    return null;
                }

                // 3.3 поиск единицы измерения
                docRecord.Unit = findObjectByGUID<Units>(item.UnitID);
                if (docRecord.Unit == null)
                {
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке документа {0}: не найдена единица измерения табличной части по guid={1}", findedElement.DocNo, item.UnitID), "Error", null);
                    //return null;  
                }
                else
                {
                    docRecord.Coeff = (decimal)docRecord.Unit.Koeff;
                }
                
                docRecord.QuantityOfUnits = item.QuantityOfUnits;
                docRecord.TotalQuantity = item.TotalQuantity;
                docRecord.BestBefore = item.BestBefore;

                // 3.4 поиск характеристики
                if (item.PropertyID!=Guid.Empty)
                {
                    docRecord.Property = findObjectByGUID<PropertiesOfGoods>(item.PropertyID);
                    if (docRecord.Property == null)
                    {
                        lokSendAndShowMessage(String.Format("Ошибка при загрузке документа Требование на разгрузку {0}: не найдена характеристика табличной части по guid={1}", record.DocNo, item.PropertyID), "Error", null);
                        return null;
                    }
                }
                findedElement.DocTrebovanieGoodsCollection.Add(docRecord);                
            }

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
         }

        private bool LoadAllDocsTrebovanie()
        {
            XPQuery<SUTZ_NET_DocTrebovanieHeaders> records = new XPQuery<SUTZ_NET_DocTrebovanieHeaders>(objectSpace.Session());
            var listOfDocs = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocTrebovanieHeaders document in listOfDocs)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Требований на разгрузку по маркеру отмены!"), "Warning", null);
                    break;
                }
                DocTrebovanie findedElement = loadOneDocTrebovanie(document);
                bool isCommited = runCommitChanges(typeof(DocTrebovanie), "Требование на разгрузку");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен документ: = {0}", findedElement), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_DocTrebovanieHeaders), 1);
                    }
                }
            }
            return true;
        }
       #endregion

        #region Загрузчик Номенклатуры
        private Goods loadOneGood(SUTZ_NET_Goods record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. Метод можно сделать обобщенным, для использования во всех методах;
            Goods good = findObjectByGUID<Goods>(record.iddSUTZ_guid);            
            if (good==null)
            {
                good = objectSpace.CreateObject<Goods>();
                good.idGUID = record.iddSUTZ_guid;
            }
            good.IddSUTZ_int  = record.iddSUTZ_int;

            int result = 0;
            if (Int32.TryParse(record.Code, out result))
            {
                good.Code = result;
            }            
            good.Article      = record.Article.Trim();
            good.FullDescription = record.FullDescription.Trim();

            // 2.2 установка разделителя учета
            good.Delimeter = findDelimiterByGUID(record.Delimeter); ;
            good.ShortDescription = record.Description.Trim();
            good.GroupExpDate = record.GroupExpDate;
            good.EnableExpDate = record.EnableExpDate;
            good.FilterOfSelect = record.FilterOfSelect;
            good.FilterOfMoving = record.FilterOfMoving;
            good.CalcByVolume = record.CalcByVolume;
            good.IsASetOfGoods = record.IsASetOfGoods;
            good.IsMarkDeleted = record.isMarkForDeleted;
            good.Comment = record.Comment.Trim();

            // 2.3 установка базовой единицы измерения
            Units unit = null;
            if (Guid.Empty!=record.BaseUnit)
            {
                unit = findObjectByGUID<Units>(record.BaseUnit);
                if (unit == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке Номенклатуры row_id = {0}, код = {1}. Не удалось найти ед.измерения по guid={2}", record.row_id, record.Code, record.BaseUnit), "Warning", null);
                }            
            }            
            good.BaseUnit = unit;

            // 2.4 установка страны производства
            Countries country = null;
            if (record.CountryOfProd!=Guid.Empty)
            {
                country = findObjectByGUID<Countries>(record.CountryOfProd);
                if (country == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке Номенклатуры row_id = {0}, код = {1}. Не удалось найти Страну по guid={2}", record.row_id, record.Code, record.CountryOfProd), "Warning", null);
                }
            }
            good.CountryOfProd = country;            

            // 2.5 установка производителя
            Manufacturers manufacturer = null;
            if (record.Manufacturer!=Guid.Empty)
            {
                manufacturer = findObjectByGUID<Manufacturers>(record.Manufacturer);
                if (manufacturer == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке Номенклатуры. row_id = {0}, код = {1}. Не удалось найти Производителя по guid={2}", record.row_id, record.Code, record.Manufacturer), "Warning", null);
                }
            }            
            good.Manufacturer = manufacturer;            

            // 2.6 установка родителя
            Goods parent = null;
            if (Guid.Empty != record.ParentId)
            {
                parent = findObjectByGUID<Goods>(record.ParentId);
                if (parent == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке Номенклатуры. row_id = {0}, код = {1}. не удалось найти Родителя по guid={2}", record.Code, record.Manufacturer, record.row_id), "Warning", null);                    
                }                
            }
            good.ParentID = parent;
            good.IsFolder = record.IsFolder;
            
            // здесь нужно сделать запись двух объектов в одной транзакции. Как?
            // так же нужно сделать анализ удачности загрузки всех реквизитов элемента. Если хотя бы один не удалось загрузить,
            // пишем в поле "is_read" вместо 1 2,3,4, и т.д до 99. 
            record.dTimeRead = DateTime.Now.ToLocalTime();            
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                good = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                good.Save();
            }
            record.Save();
            return good;
        }

        // метод для загрузки всех записей по справочнику номенклатуры
        private bool LoadAllGoods()
        {             
            XPQuery<SUTZ_NET_Goods> records =  new XPQuery<SUTZ_NET_Goods>(objectSpace.Session());
            var listOfGoods = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Goods good in listOfGoods)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Номенклатуры по маркеру отмены!"), "Warning", null);
                    break;
                }
                Goods findedElement = loadOneGood(good);
                bool isCommited = runCommitChanges(typeof(Goods), "Номенклатура");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен товар: ({0}) {1}", findedElement.Code, findedElement.ShortDescription), 
"Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_Goods), 1);
                    }
                }
            }
            return true;
        }
       #endregion

        #region Загрузчик Единиц измерения
        private Units loadOneUnitOFGoods(SUTZ_NET_Units record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            Units findedElement = findObjectByGUID<Units>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Units>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }           
 
            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Description = "";// record.Description.Trim();
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 1.3 установка классификатора единицы измерения             
            OKEI okei = null;
            if (Guid.Empty != record.OKEI)
            {
                okei = findObjectByGUID<OKEI>(record.OKEI);
                if (okei == null)
                {
                    hasError += 1;
                    String strMessage = String.Format("Ошибка при загрузке ЕдиницыИзмерения. row_id = {0}, код = {1}. не удалось найти ОКЕИ по guid={2}", record.row_id, record.Code, record.OKEI);
                    lokSendAndShowMessage(strMessage, "Warning", null);
                }
            }

            findedElement.OKEI = okei;
            findedElement.Koeff = record.Coefficient;
            findedElement.Height = (float)record.Height;
            findedElement.Width = (float)record.Width;
            findedElement.Depth =  (float)record.Depth;
            findedElement.Weight =  (float)record.Weight;
            findedElement.Barcode = record.Barcode;

             // 2.6 установка родителя
            Goods parent = findObjectByGUID<Goods>(record.ParentID);
            if (Guid.Empty != record.ParentID)
            {
                parent = findObjectByGUID<Goods>(record.ParentID);
                if (parent == null)
                {
                    hasError += 1;
                    String strMessage = String.Format("Ошибка при загрузке ЕдиницыИзмерения. row_id = {0}, код = {1}. не удалось найти Родителя по guid={2}", record.row_id, record.Code, record.OKEI);
                    lokSendAndShowMessage(strMessage, "Warning", null);
                }
            }

            record.dTimeRead = DateTime.Now.ToLocalTime();           
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
                parent.UnitsCollection.Add(findedElement);
            }                    
            record.Save();
            return findedElement;
        }

        // метод для загрузки всех записей по справочнику номенклатуры
        private bool LoadAllUnits()
        {
            XPQuery<SUTZ_NET_Units> records = new XPQuery<SUTZ_NET_Units>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Units unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Единиц измерения по маркеру отмены!"), "Warning", null);
                    break;
                }
                Units findedelement = loadOneUnitOFGoods(unit);
                bool isCommited = runCommitChanges(typeof(Units), "Единица измерения");
                if (findedelement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружена Единица измерения. row_id = {0}, единица:{1} товар:{2}", unit.row_id, findedelement.Description, findedelement.ParentId.ShortDescription), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_Units), 1);
                    }
                }
            }
            
            return true;
        }
        #endregion

        #region Загрузчик Кладовых
        private StockRooms loadOneStockRoom(SUTZ_NET_StockRooms record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            StockRooms findedElement = findObjectByGUID<StockRooms>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<StockRooms>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учет
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.TypeOfStockRoom = (Enum.IsDefined(typeof(enTypeOfStockRooms), record.TypeOfStockRoom) ? (enTypeOfStockRooms)record.TypeOfStockRoom : enTypeOfStockRooms.НеВыбран);
            findedElement.isGroupOfStockRooms = record.isGroup;
            findedElement.LogisticParameter = record.logisticParameter;
            findedElement.RemoveFromMixing = record.removeFromMix;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // здесь нужно сделать запись двух объектов в одной транзакции. Как?
            // так же нужно сделать анализ удачности загрузки всех реквизитов элемента. Если хотя бы один не удалось загрузить,
            // пишем в поле "is_read" вместо 1 2,3,4, и т.д до 99. 
            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read>253)? (byte)0: ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();           
            return findedElement;
         }

        // метод для загрузки всех записей по справочнику кладовых
        private bool LoadAllStockrooms()
        {
            XPQuery<SUTZ_NET_StockRooms> records = new XPQuery<SUTZ_NET_StockRooms>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_StockRooms unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Кладовых по маркеру отмены!"), "Warning", null);
                    break;
                }

                StockRooms findedElement = loadOneStockRoom(unit);
                bool isCommited = runCommitChanges(typeof(StockRooms), "Кладовые");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника Кладовые: row_id = {0}, наименование = {1}", unit.row_id, findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_StockRooms), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Кодов размещения
        private StorageCodes loadOneStorageCode(SUTZ_NET_StorageCodes record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            StorageCodes findedElement = findObjectByGUID<StorageCodes>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<StorageCodes>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }     
            // установка 
            if (findedElement.lokSession==null)
            {
                findedElement.lokSession = (Session)objectSpace;
            }

            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.IsFolder = record.IsFolder;
            // установка родителя
            findedElement.ParentID = findObjectByGUID<StorageCodes>(record.ParentID);
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // установка кладовой ячейки
            if (record.StockRoomID != Guid.Empty)
            {
                StockRooms stockRoom = findObjectByGUID<StockRooms>(record.StockRoomID);
                findedElement.Stockroom = stockRoom;
                if (stockRoom == null)
                {
                    hasError += 1;
                    lokSendAndShowMessage(String.Format("Ошибка при загрузке КодРазмещения row_id = {0}, код = {1}. Не удалось найти кладовую по guid={2}", record.row_id, findedElement.Code, record.StockRoomID), "Warning", null);
                }
            }

            findedElement.Stillage  = record.RackNo;
            findedElement.Floor     = record.Floor;
            findedElement.Cell      = record.Cell;
            findedElement.SubCell   = record.SubCell;
            // определение типа объема
            findedElement.CellType = findedElement.getTypeOfCellBySize(record.Height, record.Width, record.Depth);
            // состояние ячейки - используется, не используется.
            if (record.CellState==1||record.CellState==2)
            {
                findedElement.CellState = (enCellStates)record.CellState;
            }
            
            // установка прописанного товара в ячейке:
            Goods good = findObjectByGUID<Goods>(record.GoodID);
            if ((record.GoodID!=Guid.Empty)&&(good==null))
            {
                hasError += 1;
                lokSendAndShowMessage(String.Format("Ошибка при загрузке КодРазмещения row_id = {0}, код = {1}. Не удалось найти товар по guid={2}", record.row_id, findedElement.Code, record.GoodID), "Warning", null);                
            }
            findedElement.Good = good;
            findedElement.MaxItems = record.MaxItems;
            record.dTimeRead = DateTime.Now.ToLocalTime();

            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();            
            return findedElement;
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllStorageCodes()
        {
            XPQuery<SUTZ_NET_StorageCodes> records = new XPQuery<SUTZ_NET_StorageCodes>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_StorageCodes unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Кодов размещения по маркеру отмены!"), "Warning", null);
                    break;
                }
                StorageCodes findedElement = loadOneStorageCode(unit);
                bool isCommited = runCommitChanges(typeof(StorageCodes), "Коды размещения");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен КодРазмещения, код = {0}", findedElement.Code), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_StorageCodes), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик ХарактеристикиНоменклатуры
        private PropertiesOfGoods loadOneProperty(SUTZ_NET_Properties record)
        {
            int hasError = 0;            
            // 1. поиск объекта по iddguid. 
            PropertiesOfGoods findedElement = findObjectByGUID<PropertiesOfGoods>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<PropertiesOfGoods>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            // 1.3 поиск владельца
            Goods goodParent = findObjectByGUID<Goods>(record.ParentID);
            if (goodParent==null)
            {
                lokSendAndShowMessage(String.Format("Не загружена характеристика номенклатуры row_id={0}, т.к. не найден родитель по guid={1}", record.row_id, record.ParentID), "Error", null);
                return null;
            }
    
            findedElement.Code = record.Code;
            findedElement.Description = record.Description;
            findedElement.ProductionDate = record.ProductionDate;
            findedElement.ExpirationDate = record.ExpirationDate;
            findedElement.PackKoeff = record.PackCoeff;  
            findedElement.Comment = record.Comment;
            record.dTimeRead = DateTime.Now.ToLocalTime();
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
                goodParent.PropertiesOfGoodsCollection.Add(findedElement);            
            }
            record.Save();            
            return findedElement;
        }

        // метод для загрузки всех записей по справочнику Характеристик номенклатуры.
        private bool LoadAllProperties()
        {
            XPQuery<SUTZ_NET_Properties> records = new XPQuery<SUTZ_NET_Properties>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Properties unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки характеристик товара по маркеру отмены!"), "Warning", null);
                    break;
                }
                PropertiesOfGoods findedElement = loadOneProperty(unit);
                bool isCommited = runCommitChanges(typeof(PropertiesOfGoods), "Характеристика номенклатуры");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружена характеристика. Товар:{0} хар-ка = {1}", findedElement.ParentId.Code,findedElement.Code), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_Properties), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Стран
        private Countries loadOneCountry(SUTZ_NET_Countries record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            Countries findedElement = findObjectByGUID<Countries>(record.iddSUTZ_guid);            
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Countries>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учета
            Delimeters delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Delimeter = delimeter;
            findedElement.Code = record.Code;
            findedElement.Description = record.Description;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();            

            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllCountries()
        {
            XPQuery<SUTZ_NET_Countries> records = new XPQuery<SUTZ_NET_Countries>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Countries unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника Страны по маркеру отмены!"), "Warning", null);
                    break;
                }
                Countries findedElement = loadOneCountry(unit);
                bool isCommeted = runCommitChanges(typeof(Countries), "Страны");
                if (findedElement!=null)
                {
                    if (isCommeted)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружена Страна, наименование = {0}", findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_Countries), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Производителей
        private Manufacturers loadOneManufacture(SUTZ_NET_Manufacturers record)
        {
            int hasError = 0;

            // 1. поиск объекта по iddguid. 
            Manufacturers findedElement = findObjectByGUID<Manufacturers>(record.iddSUTZ_guid);            
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Manufacturers>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();            

            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllManufacturers()
        {
            XPQuery<SUTZ_NET_Manufacturers> records = new XPQuery<SUTZ_NET_Manufacturers>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Manufacturers unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника Производителей по маркеру отмены!"), "Warning", null);
                    break;
                }
                Manufacturers findedElement = loadOneManufacture(unit);
                bool isCommited = runCommitChanges(typeof(Manufacturers), "Производители");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен Производитель, наименование = {0}", findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_Manufacturers), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Виды работ
        private JobTypes loadOneJobType(SUTZ_NET_JobTypes record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            JobTypes findedElement = findObjectByGUID<JobTypes>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<JobTypes>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);

            int lokInt = 0;
            bool boolResult = Int32.TryParse(record.Code, out lokInt);
            findedElement.Code = lokInt;

            findedElement.Description = record.Description;
            findedElement.IsEnabled = record.IsEnabled;
            findedElement.TypeOfWork = (enTypeOfWorks)record.TypeOfWork;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }

        // метод для загрузки всех записей по справочнику видов работ
        private bool LoadAlljobTypes()
        {
            XPQuery<SUTZ_NET_JobTypes> records = new XPQuery<SUTZ_NET_JobTypes>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_JobTypes unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Видов работ по маркеру отмены!"), "Warning", null);
                    break;
                }
                JobTypes findedElement = loadOneJobType(unit);
                bool isCommited = runCommitChanges(typeof(JobTypes), "Справочник Виды работы");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен вид работы, наименование = {0}", findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_JobTypes), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик Контрагентов
        private Clients loadOneClient(SUTZ_NET_Clients record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            Clients findedElement = findObjectByGUID<Clients>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Clients>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 1.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.FullDescription = record.FullDescription;
            findedElement.DeliveryAdress = record.DeliveryAdress;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else 
            { 
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }

        // Загрузка элемента справочника Клиентов:
        private bool LoadAllClients()
        {
            XPQuery<SUTZ_NET_Clients> records = new XPQuery<SUTZ_NET_Clients>(((XPObjectSpace)objectSpace).Session);
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Clients client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника Контрагентов по маркеру отмены!"), "Warning", null);
                    break;
                }
                Clients findedElement = loadOneClient(client);
                bool isCommited = runCommitChanges(typeof(Clients), "Справочник Клиенты");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника Контрагенты row_id = {0}, наименование = {1}", client.row_id, findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_Clients), 1);
                    }
                }
            }
            return true;
        }
        #endregion

        #region Загрузчик справочника Логистика
        // метод загружает параметры Логистики
        private bool LoadLogistics()
        {
            int hasError = 0;

            XPQuery<SUTZ_NET_Logistics> records = new XPQuery<SUTZ_NET_Logistics>(objectSpace.Session());
            XPQuery<LogisticsSettings> findedQuery = new XPQuery<LogisticsSettings>(objectSpace.Session());

            var listOfElements = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Logistics element in listOfElements)
            {
                // поиск элемента по коду элемента.
                var findedElementQuery = from aa in findedQuery where aa.Code == element.Code && aa.Delimeter.DelimeterID == element.Delimeter select aa;

                LogisticsSettings findedElement = null;
                int createNew = 0;
                if (findedElementQuery == null)
                {
                    createNew = 1;
                }
                else if(findedElementQuery.Count()==0) 
                {
                    createNew = 1;
                }

                if (createNew==1)
                {
                    findedElement = objectSpace.CreateObject<LogisticsSettings>();
                    findedElement.Code = element.Code;
                }
                else
                {
                    findedElement = findedElementQuery.First<LogisticsSettings>();
                }
                findedElement.Delimeter = findDelimiterByGUID(element.Delimeter);
                findedElement.Description = element.Description;
                findedElement.LogisticValue = element.LogisticValue;
                element.dTimeRead = DateTime.Now.ToLocalTime();
                if (hasError > 0)
                {
                    element.is_read = (element.is_read == 0) ? (byte)2 : ((element.is_read > 253) ? (byte)0 : ((byte)(element.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                    findedElement = null;
                }
                else
                {
                    element.is_read = 1;
                    element.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                    findedElement.Save();
                }                
                element.Save();
                bool isCommited = runCommitChanges(typeof(LogisticsSettings), "Справочник Логистика");
                if (findedElement!=null)
                {
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен логистический параметр = {0}, значение={1}", element.Code, element.LogisticValue), "Trace", null);
                    }
                }                
            }            
            return true;
        }
        #endregion

        #region Загрузчик справочника ПодразделенияКлиента
        private ClientDivisions loadOneCustomerDivision(SUTZ_NET_CustomerUnits record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            ClientDivisions findedElement = findObjectByGUID<ClientDivisions>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<ClientDivisions>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.Address = record.AdressSubDivision;
            findedElement.KPP = record.KppSubDivision;
            findedElement.Comment = record.Comment;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            // 2.3 поиск владельца
            Clients clientParent = findObjectByGUID<Clients>(record.ParentID);
            if (clientParent == null)
            {
                lokSendAndShowMessage(String.Format("Не загружен магазин клиента row_id={0}, т.к. не найден родитель по guid={1}", record.row_id, record.ParentID), "Error", null);
                return null;
            }
            clientParent.ClientDivisionsCollection.Add(findedElement);
            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement.Save();
            }
            else 
            {
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                record.is_read = 1;
                findedElement = null;
            }            
            record.Save();            
            return findedElement;
        }

        private bool LoadAllCustomerDivisions()
        {
            XPQuery<SUTZ_NET_CustomerUnits> records = new XPQuery<SUTZ_NET_CustomerUnits>(objectSpace.Session());
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_CustomerUnits client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Подразделений клиента по маркеру отмены!"), "Warning", null);
                    break;
                }
                ClientDivisions findedElement = loadOneCustomerDivision(client);
                bool isCommited = runCommitChanges(typeof(ClientDivisions), "Справочник Подразделения клиента");
                if (findedElement!=null)
                {                    
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника ПодразделенияКлиента row_id = {0}, наименование = {1}", client.row_id, findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_CustomerUnits), 1);
                    }                    
                }                
            }
            return true;
        }
       #endregion

        #region Загрузчик справочника МагазиныКлиента
        private ClientShops loadOneCustomerStore(SUTZ_NET_CustomerStores record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            ClientShops findedElement = findObjectByGUID<ClientShops>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<ClientShops>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.Address = record.AdressSubDivision;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;
            // 2.3 поиск владельца
            Clients clientParent = findObjectByGUID<Clients>(record.ParentID);
            if (clientParent == null)
            {
                lokSendAndShowMessage(String.Format("Не загружен магазин клиента row_id={0}, т.к. не найден родитель по guid={1}", record.row_id, record.ParentID), "Error", null);
                return null;
            }
            clientParent.ClientShopsCollection.Add(findedElement);
            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else 
            { 
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();
            return findedElement;
        }

        private bool LoadAllCustomerStories()
        {
            XPQuery<SUTZ_NET_CustomerStores> records = new XPQuery<SUTZ_NET_CustomerStores>(objectSpace.Session());
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_CustomerStores client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Магазинов клиента по маркеру отмены!"), "Warning", null);
                    break;
                }
                ClientShops findedElement = loadOneCustomerStore(client);
                bool isCommited = runCommitChanges(typeof(ClientShops), "Справочник Магазины клиента");
                if (findedElement!=null)
                {                    
                    if (isCommited)
                    {
                        lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника МагазинКлиента row_id = {0}, наименование = {1}", client.row_id, findedElement.Description), "Trace", null);
                        lokSendMessageForObjectType(typeof(SUTZ_NET_CustomerStores), 1);
                    }
                }                
            }
            return true;
        }
       #endregion

        #region Загрузчик справочника Склады
       private Warehouses loadOneWarehouse(SUTZ_NET_Warehouses record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            Warehouses findedElement = findObjectByGUID<Warehouses>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Warehouses>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.OwnWarehouse = record.OwnWarehouse;
            findedElement.IsEnable = record.IsEnable;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else 
            {
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                record.is_read = 1;
                findedElement.Save();
            }            
            record.Save();            
            return findedElement;
        }

        // загрузка всех складов АСУ
        private bool LoadAllWarehouses()
        {
            XPQuery<SUTZ_NET_Warehouses> records = new XPQuery<SUTZ_NET_Warehouses>(objectSpace.Session());
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Warehouses client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage("Отмена загрузка справочника Складов по маркеру отмены!", "Warning", null);
                    break;
                }
                Warehouses findedElement = loadOneWarehouse(client);
                bool isCommeted = runCommitChanges(typeof(Warehouses), "Справочник Склады");
                if ((findedElement!=null)&&(isCommeted))
                {
                  lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника Склады row_id = {0}, наименование = {1}", client.row_id, findedElement.Description), "Trace", null);
                  lokSendMessageForObjectType(typeof(SUTZ_NET_Warehouses), 1);
                }                
            }
            return true;
        }
       #endregion

        #region Загрузчик справочника ОКЕИ
        private OKEI loadOneOKEI(SUTZ_NET_OKEI record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            OKEI findedElement = findObjectByGUID<OKEI>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<OKEI>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.InternationalDescription = record.Description;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else 
            { 
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }            
            record.Save();            
            return findedElement;
        }

        private bool LoadAllOKEI()
        {
            XPQuery<SUTZ_NET_OKEI> records = new XPQuery<SUTZ_NET_OKEI>(objectSpace.Session());
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_OKEI client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника ОКЕИ товара по маркеру отмены!"), "Warning", null);
                    break;
                }
                OKEI findedElement = loadOneOKEI(client);
                bool isCommited = runCommitChanges(typeof(OKEI), "Справочник ОКЕИ");
                if ((findedElement != null) && (isCommited))
                {
                    lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника ОКЕИ row_id = {0}, наименование = {1}", client.row_id, findedElement.Description), "Trace", null);
                    lokSendMessageForObjectType(typeof(SUTZ_NET_OKEI), 1);
                }                
            }
            return true;
        }
       #endregion

        #region Загрузчик справочника Штрихкоды номенклатуры
        private BarcodesOfGoods loadOneBarcode(SUTZ_NET_BarcodesOfGoods record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            BarcodesOfGoods findedElement = findObjectByGUID<BarcodesOfGoods>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<BarcodesOfGoods>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Barcode = record.Barcode.Trim();

            // 2.3 установка владельца товара для штрихокода
            Goods good = null;
            if (Guid.Empty != record.GoodID)
            {
                good = findObjectByGUID<Goods>(record.GoodID);
                if (good == null)
                {
                    hasError += 1;
                    String strMessage = String.Format("Ошибка при загрузке Штрихкода. row_id = {0}, штрихкод = {1}. не удалось найти Товар по guid={2}", record.row_id, record.Barcode, record.GoodID);
                    lokSendAndShowMessage(strMessage, "Warning", null);
                }
            }
            findedElement.ParentId = good;

            Units unit = null;
            if (Guid.Empty != record.UnitID)
            {
                unit = findObjectByGUID<Units>(record.UnitID);
                if (unit == null)
                {
                    hasError += 1;
                    String strMessage = String.Format("Ошибка при загрузке Штрихкода. row_id = {0}, штрихкод = {1}. не удалось найти ЕдиницаИзмерения по guid={2}", record.row_id, record.Barcode, record.UnitID);
                    lokSendAndShowMessage(strMessage, "Warning", null);
                }
            }

            findedElement.Unit = unit;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();
            
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();
            }
            record.Save();
            return findedElement;
        }
        private bool LoadAllBarcodeOfGoods()
        {
            XPQuery<SUTZ_NET_BarcodesOfGoods> records = new XPQuery<SUTZ_NET_BarcodesOfGoods>(objectSpace.Session());
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_BarcodesOfGoods client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки Штрих-кодов товара по маркеру отмены!"), "Warning", null);
                    break;
                }
                BarcodesOfGoods findedElement = loadOneBarcode(client);
                bool isCommited = runCommitChanges(typeof(BarcodesOfGoods), "Справочник Штрихкоды Номенклатуры");
                if ((findedElement!=null)&&(isCommited))
                {
                 lokSendAndShowMessage(String.Format("Успешно загружен Штрихкод = {0}, товар: ({1}) {2}", findedElement.Barcode, findedElement.ParentId.Code, findedElement.ParentId.ShortDescription), "Trace", null);
                 lokSendMessageForObjectType(typeof(SUTZ_NET_BarcodesOfGoods), 1);
                }               
            }
            return true;
        }
      #endregion

        #region Справочник СкладскиеДокументы
        // "isNeedToOutbound" - в параметре возвращается признак, что записанный элемент нужно выгружать обратно в 1С.
        // создание структуры для анализа вида работы по документу до загрузки и после загрузки элемента.
        private struct structDocJobeStatus
        {
            public enTypesOfMobileStates? oldType;
            public enTypesOfMobileStates? newType;
            public bool isNeedToOutbound;
        };

        private DocJobesProperties loadOneDocJobeProperty(SUTZ_NET_DocJobeProperties record, ref structDocJobeStatus structDocJobe)
        {
            int hasError = 0; 

            // 1. поиск объекта класса Свойств-ВидовРабот по документу и виду работы:
            DocJobesProperties findedElement = findObjectByGUID<DocJobesProperties>(record.iddSUTZ_GUID);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<DocJobesProperties>();
                findedElement.idGUID = record.iddSUTZ_GUID;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);

            // 2.3 поиск документа по GUID:
            BaseDocument findBaseDoc = findObjectByGUID<BaseDocument>(record.DocBaseID);
            if (findBaseDoc == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при загрузке складского документа row_id={0}. Не найден BaseDocument по guid={1}", record.row_id, record.DocBaseID), "Error", null);
                hasError += 1;
            }
            findedElement.BaseDoc = findBaseDoc;
            // 2.4 поиск перечисления СУТЗ.NET по виду операции моб.сутз:
            if (Enum.IsDefined(typeof(enTypesOfMobileStates), record.MobileStatus))
            {
                // 2.4.1 запомним исходное и новое значение типа работы в переданной структуре:
                structDocJobe.oldType = findedElement.TypeOfMobileState;
                structDocJobe.newType = (enTypesOfMobileStates)record.MobileStatus;

                findedElement.TypeOfMobileState = (enTypesOfMobileStates)record.MobileStatus;
                // 2.4.2 если СУТЗ отправляет статус ВПроцессеОтправки, то мы ставим статус ДоступенДляРаботы и отвечаем обратно в 1С:
                if (findedElement.TypeOfMobileState==enTypesOfMobileStates.ВПроцессеОтправки)
                {
                    findedElement.TypeOfMobileState = enTypesOfMobileStates.ДоступенДляРаботы;
                    structDocJobe.isNeedToOutbound = true;
                }
                else if (findedElement.TypeOfMobileState==enTypesOfMobileStates.ВПроцессеОтзыва)                    
                {
                    findedElement.TypeOfMobileState = enTypesOfMobileStates.ОтозванИзРаботы;
                    structDocJobe.isNeedToOutbound = true;
                }
            }
            else
            {
                lokSendAndShowMessage(String.Format("Ошибка при загрузке складского документа row_id={0}. Не найден удалось конвертировать значение {1} в перечисление enTypesOfMobileStates", record.row_id, record.MobileStatus), "Error", null);
                hasError += 1;
            }

            // 2.5 поиск вида работы 
            JobTypes findedJobeType = findObjectByGUID<JobTypes>(record.JobeType);
            if (findedJobeType == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при загрузке складского документа row_id={0}. Не найден JobType по guid={1}", record.row_id, record.JobeType), "Error", null);
                hasError += 1;
            }
            findedElement.JobType = findedJobeType;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();            
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findedElement.Save();// запись таблицы свойств документа
                record.is_read = 1;                
            }            
            record.Save();// запись таблицы обмена                        
            return findedElement;
        }
        
        // метод для загрузки всех записей по справочнику Документ-ВидРаботы
        private bool LoadAllDocJobeProperties()
        {
            structDocJobeStatus strdocJobeStatus = new structDocJobeStatus();

            XPQuery<SUTZ_NET_DocJobeProperties> records = new XPQuery<SUTZ_NET_DocJobeProperties>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocJobeProperties unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника Документ-видРаботы товара по маркеру отмены!"), "Warning", null);
                    break;
                }

                //bool isNeedToOutbound = false;
                strdocJobeStatus.isNeedToOutbound = false;
                strdocJobeStatus.oldType = null;
                strdocJobeStatus.newType = null;

                DocJobesProperties elementDocJobe = loadOneDocJobeProperty(unit, ref strdocJobeStatus);

                // анализ старого и нового статусов:
                if ((strdocJobeStatus.oldType!=null)&&(strdocJobeStatus.newType!=null))
                {
                    if (strdocJobeStatus.oldType!=strdocJobeStatus.newType)
                    {
                        // вызов модуля работы с заданиями для моб.сутз:
                        ControlWorkUnitsByDocAndJobe controlUnits = new ControlWorkUnitsByDocAndJobe(objectSpace);
                        controlUnits.sendMessage += lokSendAndShowMessage;
                        bool resultProcessJobType = controlUnits.proccessDoc(elementDocJobe, (enTypesOfMobileStates)strdocJobeStatus.oldType, (enTypesOfMobileStates)strdocJobeStatus.newType);

                        if (!resultProcessJobType)
                        {
                            lokSendAndShowMessage(String.Format("Отмена загрузки справочника Документ-ВидРаботы из-за ошибки модуля контроля заданий для моб.сутз row_id={0}",unit.row_id), "Warning", null);
                            continue;
                        }
                    }
                }

                bool isCommited = runCommitChanges(typeof(DocJobesProperties), "Справочник Складские документы (DocJobeProperties)");
                if (elementDocJobe==null)
                {
                    continue;
                }
                if (isCommited)
                {
                    String strMessage = String.Format("Успешно загружен документ-вид работы: {0}-{1} ", elementDocJobe.BaseDoc.DocNo, elementDocJobe.JobType.Description);
                    lokSendAndShowMessage(strMessage, "Trace", null);
                    lokSendMessageForObjectType(typeof(SUTZ_NET_DocJobeProperties), 1);
                }
                // при успешной загрузке элемента: 
                //  - поменять статус вида работы на Доступен
                //  - выгрузить в СУТЗ обратно элемент с признаком Доступен
                if (isCommited && strdocJobeStatus.isNeedToOutbound && (elementDocJobe!=null))
                {
                    Users current_User = (Users)SecuritySystem.CurrentUser;
                    Users currentUser = objectSpace.Session().FindObject<Users>(new BinaryOperator("idd", current_User.idd, BinaryOperatorType.Equal));

                    NET_SUTZ_Unloads unloads = new NET_SUTZ_Unloads(objectSpace.Session());
                    unloads.ObjectType = typeof(DocJobesProperties).FullName;
                    unloads.ObjectRow_Id = elementDocJobe.idd;
                    unloads.Outload = 0;
                    unloads.User = currentUser;
                    unloads.dTimeInsertRecord = DateTime.Now.ToLocalTime();
                    unloads.dTimeUnloadRecord = null;
                    unloads.FullDocTable = false;
                    unloads.Save();
                    isCommited = runCommitChanges(typeof(NET_SUTZ_Unloads), String.Format("DocJobeProperties: документ: {0} по виду работы: {1} установлен статус:{2}. В СУТЗ 1С:7.7 отправлено подтверждение по виду работы.", elementDocJobe.BaseDoc.DocNo, elementDocJobe.JobType.Description, elementDocJobe.TypeOfMobileState));
                }
            }
            return true;
        }
        #endregion

        #region Справочник СвойстваДокумента
        private BaseDocument loadOneDocProperty(SUTZ_NET_DocProperties record)
        {
            int hasError = 0;

            // 2.3 поиск документа по GUID:
            BaseDocument findBaseDoc = findObjectByGUID<BaseDocument>(record.DocBaseID);
            if (findBaseDoc == null)
            {
                lokSendAndShowMessage(String.Format("Ошибка при загрузке свойства документа row_id={0}. Не найден BaseDocument по guid={1}", record.row_id, record.DocBaseID), "Error", null);
                return null;// нет смысла дальше выполнять загрузку.
            }

            // 3. загрузка в базовый документ новых значений знаков документа с первого по четвертый:
            if (Enum.IsDefined(typeof(enDocPropertyStatus), (byte)record.Sign1))
            {
                findBaseDoc.FirstSign = (enDocPropertyStatus)record.Sign1;
            }
            else
            {
                findBaseDoc.FirstSign = enDocPropertyStatus.НеВыбран;
            }
            // 3.2 второй знак
            if (Enum.IsDefined(typeof(enDocPropertyStatus), (byte)record.Sign2))
            {
                findBaseDoc.SecondSign = (enDocPropertyStatus)record.Sign2;
            }
            else
            {
                findBaseDoc.SecondSign = enDocPropertyStatus.НеВыбран;
            } 
            // 3.3 третий знак
            if (Enum.IsDefined(typeof(enDocPropertyStatus), (byte)record.Sign3))
            {
                findBaseDoc.ThirdSign = (enDocPropertyStatus)record.Sign3;
            }
            else
            {
                findBaseDoc.ThirdSign = enDocPropertyStatus.НеВыбран;
            } 
            // 3.4 четвертый знак
            if (Enum.IsDefined(typeof(enDocPropertyStatus), (byte)record.Sign4))
            {
                findBaseDoc.FourthSign = (enDocPropertyStatus)record.Sign4;
            }
            else
            {
                findBaseDoc.FourthSign = enDocPropertyStatus.НеВыбран;
            }
            
            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findBaseDoc = null;
            }
            else
            {
                record.is_read = 1;
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                findBaseDoc.Save();// запись
            }
            record.Save();// запись таблицы обмена                 
            return findBaseDoc;
        }

        // метод для загрузки всех записей по справочнику СвойстваДокумента
        private bool LoadAllDocProperties()
        {
            XPQuery<SUTZ_NET_DocProperties> records = new XPQuery<SUTZ_NET_DocProperties>(objectSpace.Session());
            var listOfUnits = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_DocProperties unit in listOfUnits)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника СвойстваДокумента по маркеру отмены!"), "Warning", null);
                    break;
                }
                BaseDocument findedElement = loadOneDocProperty(unit);
                bool isCommited = runCommitChanges(typeof(BaseDocument), "доп. характеристики BaseDocument");
                if ((findedElement!=null)&&(isCommited))
                {
                    String strMessage = String.Format("Успешно загружено свойство документа: row_id:{0} документ:{1}", unit.row_id, findedElement.DocNo);
                    lokSendAndShowMessage(strMessage, "Trace", null);
                    lokSendMessageForObjectType(typeof(SUTZ_NET_DocProperties), 1);
                }                
            }
            return true;
        }
        #endregion

        #region Загрузчик справочника Пользователи
       private Users loadOneUser(SUTZ_NET_Users record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            if (record.iddSUTZ_GUID == null)
            {
                return null;
            }
            Users findedElement = findObjectByGUID<Users>(record.iddSUTZ_GUID);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Users>();
                findedElement.idGUID = record.iddSUTZ_GUID;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter         = findDelimiterByGUID(record.Delimeter);
            findedElement.DefaultDelimeter  = findedElement.Delimeter;
            findedElement.UserName          = record.UserName;
            findedElement.FullUserName      = record.FullUserName;
            findedElement.FIOBage           = record.FIO_Bage;
            findedElement.IsMarkDeleted     = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();

            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else 
            {
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                record.is_read = 1;
                findedElement.Save();
            }            
            record.Save();           
            return findedElement;
        }

        private bool LoadAllUsers()
        {
            XPQuery<SUTZ_NET_Users> records = new XPQuery<SUTZ_NET_Users>(objectSpace.Session());
            var listOfElements = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_Users element in listOfElements)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage(String.Format("Отмена загрузки справочника Пользователи по маркеру отмены!"), "Warning", null);
                    break;
                }

                Users findedElement = loadOneUser(element);
                bool isCommited = runCommitChanges(typeof(Users), "Справочник Пользователи");
                if ((findedElement!=null)&&(isCommited))
                {
                    lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника Пользователи row_id = {0}, наименование = {1}", findedElement.idd, findedElement.UserName), "Trace", null);
                    lokSendMessageForObjectType(typeof(SUTZ_NET_Users), 1);
                }             
            }
            return true;
        }
       #endregion

        #region Загрузчик справочника СтатусыОпераций
        private MobileErrors loadOneMobileError(SUTZ_NET_OperationState record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            MobileErrors findedElement = findObjectByGUID<MobileErrors>(record.iddSUTZ_guid);
            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<MobileErrors>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }
            // 2.2 установка разделителя учета
            findedElement.Delimeter = findDelimiterByGUID(record.Delimeter);
            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            findedElement.FullDescription = record.Description.Trim();
            findedElement.IsEnable = record.Is_Enabled;
            findedElement.IsDone = record.Is_Done;
            findedElement.IsError = record.Is_Error;
            findedElement.IsMarkDeleted = record.isMarkForDeleted;

            record.dTimeRead = DateTime.Now.ToLocalTime();
            if (hasError > 0)
            {
                record.is_read = (record.is_read == 0) ? (byte)2 : ((record.is_read > 253) ? (byte)0 : ((byte)(record.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                findedElement = null;
            }
            else
            {
                record.UserHostReader = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator("row_id", currentSessionSettings.CurrentHostUser.row_id));
                record.is_read = 1;
                findedElement.Save();
            }
            record.Save();
            return findedElement;
        }

        // загрузка всех Статусов операций моб.сутз (Ошибки)
        private bool LoadAllOperationStates()
        {
            XPQuery<SUTZ_NET_OperationState> records = new XPQuery<SUTZ_NET_OperationState>(objectSpace.Session());
            var listOfClients = from c in records orderby c.row_id where c.is_read != 1 select c;
            foreach (SUTZ_NET_OperationState client in listOfClients)
            {
                if (clToken.IsCancellationRequested)
                {
                    lokSendAndShowMessage("Отмена загрузка справочника Статусы операций по отмены!", "Warning", null);
                    break;
                }
                MobileErrors findedElement = loadOneMobileError(client);
                bool isCommeted = runCommitChanges(typeof(MobileErrors), "Справочник MobileErrors");
                if ((findedElement != null) && (isCommeted))
                {
                    lokSendAndShowMessage(String.Format("Успешно загружен элемента справочника MobileErrors row_id = {0}, наименование = {1}", client.row_id, findedElement.Description), "Trace", null);
                    lokSendMessageForObjectType(typeof(SUTZ_NET_OperationState), 1);
                }
            }
            return true;
        }
        #endregion


        private void setItemForListRow(Type ofType, int rowCount, ref int totalItems)
        {
            var item = listOfTypeMessages.FirstOrDefault(x => x.objType == ofType);
            if (item != null)
            {
                item.forInbound = rowCount;
            }
            totalItems += rowCount;
        }

        public int calculateAllInboudRows()
        {
            int totalItems = 0;
            int lokalItems = 0;

            // 1. расчет справочников: 
            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Goods>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Goods), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Units>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Units), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_StockRooms>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_StockRooms), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_StorageCodes>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_StorageCodes), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Properties>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Properties), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Countries>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Countries), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Manufacturers>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Manufacturers), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_JobTypes>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_JobTypes), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Clients>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Clients), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Logistics>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Logistics), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_CustomerUnits>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_CustomerUnits), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_CustomerStores>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_CustomerStores), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Warehouses>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Warehouses), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_OKEI>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_OKEI), lokalItems, ref totalItems);
            
            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_BarcodesOfGoods>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_BarcodesOfGoods), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocJobeProperties>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocJobeProperties), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocProperties>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocProperties), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_Users>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_Users), lokalItems, ref totalItems); 
            
            // 2. расчет документов:
            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocSpecRashodaHeaders>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocSpecRashodaHeaders), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocSpecPeremeshHeaders>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocSpecPeremeshHeaders), lokalItems, ref totalItems);
            
            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocInventoryHeaders>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocInventoryHeaders), lokalItems, ref totalItems);
            
            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocSpecPrihodaHeaders>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocSpecPrihodaHeaders), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocInvoiceOrderHeaders>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocInvoiceOrderHeaders), lokalItems, ref totalItems);

            lokalItems = (Task.Factory.StartNew<int>(() => new XPQuery<SUTZ_NET_DocTrebovanieHeaders>(objectSpace.Session()).Count(p => p.is_read == 0))).Result;
            setItemForListRow(typeof(SUTZ_NET_DocTrebovanieHeaders), lokalItems, ref totalItems); 
            
            return totalItems;
        }


        #region Члены INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
