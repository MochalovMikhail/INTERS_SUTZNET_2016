using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using DevExpress.Xpo;
//using SUTZ_2.Module.BO.References;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using System.Collections;
using DevExpress.ExpressApp.Xpo;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Exchange.SUTZ1C_SUTZNET.ndk_ecxhange_SUTZ_NET;

namespace SUTZ_2.Exchange.SUTZ1C_SUTZNET
{
   // [DomainComponent]
    [NavigationItem("Обработки"), ImageName("BO_Report")]
    [XafDisplayName("Загрузка из СУТЗ 1С:7.7")]
    [NonPersistent]
    public class SQL_Exchange_Inbound: INotifyPropertyChanged
    {
        // не визуальные компоненты
        [VisibleInDetailView(false),VisibleInListView(false)]
        private IObjectSpace objectSpace;

        [VisibleInDetailView(false), VisibleInListView(false)]
        private int intCurrentUserHostID = 0;

        // визуальные компоненты:

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

        // 9. окно сообщений о загрузке
        private string messageWindow;
        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("AllowEdit", "False")]
       // [DisplayName(@"Код")]
        public string MessageWindow
        {
            get { return messageWindow; }
            set { 
                if (messageWindow!=value)
                {
                    messageWindow = value; 
                    OnPropertyChanged("MessageWindow");
                }
            }
        }

        // конструктор по умолчанию
        public SQL_Exchange_Inbound()
        {
            IsLoadStockRooms = true;
            isLoadGoods = true;
        }

        // конструктор с делегатом
        delegateShowMessage delegateForShowMessage;
        public SQL_Exchange_Inbound(delegateShowMessage sender, IObjectSpace objSpace) : this()
        {
            delegateForShowMessage = sender;
            objectSpace = objSpace;
        }

        // метод выполняет загрузку всех данных из СУТЗ 1С:7.7
        public bool InboundAllData()
        {
            bool isLoading = true;
            //1. определение текущего пользователя 
            detectCurrentUserHost();

            // загрузка кладовых
            if (IsLoadStockRooms)
            {
                isLoading = LoadAllStockrooms();
            }

            // загрузка кодов размещения
            if (IsLoadStorageCodes)
            {
                isLoading = LoadAllStorageCodes();
            }

            // 2. загрузка справочника номенклатуры и всех связанных справочников:
            if (IsLoadManufacturers)
            {
                isLoading = LoadAllManufacturers();
            }
            if (IsLoadCountries)
            {
                isLoading = LoadAllCountries();
            }

            if (isLoadGoods)
            {
                isLoading = LoadAllGoods();
            }

            // 2.1 загрузка подчиненных единиц измерения
            if (IsLoadUnitOfGoods)
            {
                isLoading = LoadAllUnits();
            }
            // 2.2 загрузка подчиненных характеристик
            if (IsLoadProperties)
            {
                isLoading = LoadAllProperties();
            }

            if (((ICollection)objectSpace.ModifiedObjects).Count>0)
            {
                objectSpace.CommitChanges();
            }

            return isLoading;
        }

        private void detectCurrentUserHost()
        {
            string hostName = Environment.MachineName;
            string userName = Environment.UserName;

            //GroupOperator oprand = GroupOperator.And(new BinaryOperator(""))
            //SUTZ_NET_HostsUsers hostUser = objectSpace.FindObject<SUTZ_NET_HostsUsers>(new BinaryOperator());
            XPQuery<SUTZ_NET_HostsUsers> qHostUser = new XPQuery<SUTZ_NET_HostsUsers>(((XPObjectSpace)objectSpace).Session);
            var currentUSER = (from c in qHostUser where c.HostName==hostName && c.UserName==userName select c).FirstOrDefault();
            if (currentUSER == null)
            {
                SUTZ_NET_HostsUsers currentUSERForRec = objectSpace.CreateObject<SUTZ_NET_HostsUsers>();
                currentUSERForRec.HostName = hostName;
                currentUSERForRec.UserName = userName;
                try
                {
                    currentUSERForRec.Save();
                    objectSpace.CommitChanges();
                }

                catch (System.Exception ex)
                {

                }
                intCurrentUserHostID = currentUSERForRec.row_id;
            }
            else
            {
                intCurrentUserHostID = currentUSER.row_id;
            }
        }

        private T findObjectByGUID<T>(Guid param)
        {
            T findedElement = objectSpace.FindObject<T>(new BinaryOperator("iddSUTZ_GUID", param, BinaryOperatorType.Equal), 
true);
            return findedElement;
        }
        //private lokTypeEnum findEnumByValue<lokTypeEnum>(int param)
        //{
        //    if (Enum.IsDefined(typeof(lokTypeEnum), param))
        //    {               

        //        Enum.TryParse<lokTypeEnum>(param.ToString(), out outParam);
        //        Enum.Parse(typeof(lokTypeEnum), param.ToString());
        //        return  outParam;
        //    }

        //}

        #region LoadingGoods
        private bool loadOneGood(SUTZ_NET_Goods record)
        {

            int hasError = 0;
            // 1. поиск объекта по iddguid. Метод можно сделать обобщенным, для использования во всех методах;
            //Goods good = Session.DefaultSession.FindObject<Goods>(new BinaryOperator("iddSUTZ_GUID", record.iddSUTZ_guid, BinaryOperatorType.Equal), true);
            Goods good = findObjectByGUID<Goods>(record.iddSUTZ_guid);
            SUTZ_NET_Goods recordForRecord = objectSpace.FindObject<SUTZ_NET_Goods>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));
            
            if (good==null)
            {
                good = objectSpace.CreateObject<Goods>();
            }
            good.idGUID = record.iddSUTZ_guid;
            good.IddSUTZ_int  = record.iddSUTZ_int;

            int result = 0;
            if (Int32.TryParse(record.Code, out result))
            {
                good.Code = result;
            }            
            good.Article      = record.Article.Trim();
            good.FullDescription = record.FullDescription.Trim();

            // 2.2 установка разделителя учета
            Delimiters delimeter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            good.Delimiter = delimeter;
            hasError += (delimeter != null) ? 0 : 1;

            good.ShortDescription = record.Description.Trim();
            good.GroupExpDate = record.GroupExpDate;
            good.EnableExpDate = record.EnableExpDate;
            good.FilterOfSelect = record.FilterOfSelect;
            good.FilterOfMoving = record.FilterOfMoving;
            good.CalcByVolume = record.CalcByVolume;
            good.IsASetOfGoods = record.IsASetOfGoods;

            // 2.3 установка базовой единицы измерения
            Units unit = findObjectByGUID<Units>(record.BaseUnit);
            good.BaseUnit = unit;
            hasError += (unit != null) ? 0 : 1;

            // 2.4 установка страны производства
            Countries country = findObjectByGUID<Countries>(record.CountryOfProd);
            good.CountryOfProd = country;
            hasError += (country != null) ? 0 : 1;

            // 2.5 установка производителя
            Manufacturers manufacturer = findObjectByGUID<Manufacturers>(record.Manufacturer);
            good.Manufacturer = manufacturer;
            hasError += (manufacturer != null) ? 0 : 1;

            good.Comment = record.Comment.Trim();

            // 2.6 установка родителя
            Goods parent = findObjectByGUID<Goods>(record.ParentId);
            good.ParentID = parent;
            hasError += (parent != null) ? 0 : 1;
            good.IsFolder = record.IsFolder;
            
            // здесь нужно сделать запись двух объектов в одной транзакции. Как?
            // так же нужно сделать анализ удачности загрузки всех реквизитов элемента. Если хотя бы один не удалось загрузить,
            // пишем в поле "is_read" вместо 1 2,3,4, и т.д до 99. 
           try
           {
               if (recordForRecord!=null)
               {
                   recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                   recordForRecord.is_read = 1;
                   recordForRecord.UserHostReader = intCurrentUserHostID;
                   recordForRecord.Save();
               }
               good.Save();
               //objectSpace.CommitChanges();
               delegateForShowMessage(String.Format("Успешно загружен товар, код = {0}",good.Code));
               return true;
           }
           catch (System.Exception ex)
           {
               return false;
           }   
        }

        // метод для загрузки всех записей по справочнику номенклатуры
        private bool LoadAllGoods()
        {             
            XPQuery<SUTZ_NET_Goods> records =  new XPQuery<SUTZ_NET_Goods>(((XPObjectSpace)objectSpace).Session);
            //objectSpace.
            //IList<SUTZ_NET_Goods> records = objectSpace.GetObjects<SUTZ_NET_Goods>();
            var listOfGoods = from c in records where c.is_read == 0 select c;
            foreach (SUTZ_NET_Goods good in listOfGoods)
            {
                bool isLoading = loadOneGood(good);
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
       #endregion

        #region LoadingUnitOfGoods
        private bool loadOneUnitOFGoods(SUTZ_NET_Units record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            Units findedElement = findObjectByGUID<Units>(record.iddSUTZ_guid);
            SUTZ_NET_Units recordForRecord = objectSpace.FindObject<SUTZ_NET_Units>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));

            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Units>();
            }
            findedElement.iddSUTZ_GUID = record.iddSUTZ_guid;
 
            // 2.2 установка разделителя учета
            Delimiters delimeter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            findedElement.Delimiter = delimeter;
            hasError += (delimeter != null) ? 0 : 1;

            findedElement.Description = record.Description.Trim();

            // 2.3 установка классификатора единицы измерения             
            OKEI okei = objectSpace.FindObject<OKEI>(new BinaryOperator("idd", record.OKEI, BinaryOperatorType.Equal), true);
            findedElement.OKEIId = okei;

            findedElement.Rate = (float)record.Coefficient;
            findedElement.Height = (float)record.Height;
            findedElement.Width = (float)record.Width;
            findedElement.Depth = (float)record.Depth;
            findedElement.Weight = record.Weight;
            findedElement.Barcode = record.Barcode;

             // 2.6 установка родителя
            Goods parent = findObjectByGUID<Goods>(record.ParentID);
            findedElement.ParentId = parent;
            hasError += (parent != null) ? 0 : 1;

            // здесь нужно сделать запись двух объектов в одной транзакции. Как?
            // так же нужно сделать анализ удачности загрузки всех реквизитов элемента. Если хотя бы один не удалось загрузить,
            // пишем в поле "is_read" вместо 1 2,3,4, и т.д до 99. 
            try
            {
                if (recordForRecord != null)
                {
                    recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                    recordForRecord.is_read = 1;
                    recordForRecord.UserHostReader = intCurrentUserHostID;
                    recordForRecord.Save();
                }
                findedElement.Save();
                //objectSpace.CommitChanges();
                delegateForShowMessage(String.Format("Успешно загружена единица измерения, наименование = {0}", findedElement.Description));
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        // метод для загрузки всех записей по справочнику номенклатуры
        private bool LoadAllUnits()
        {
            XPQuery<SUTZ_NET_Units> records = new XPQuery<SUTZ_NET_Units>(((XPObjectSpace)objectSpace).Session);
            var listOfUnits = from c in records where c.is_read == 0 select c;
            foreach (SUTZ_NET_Units unit in listOfUnits)
            {
                bool isLoading = loadOneUnitOFGoods(unit);
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
        #endregion

        #region LoadingStockRooms
        private bool loadOneStockRoom(SUTZ_NET_StockRooms record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            StockRooms findedElement = findObjectByGUID<StockRooms>(record.iddSUTZ_guid);
            SUTZ_NET_StockRooms recordForRecord = objectSpace.FindObject<SUTZ_NET_StockRooms>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));

            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<StockRooms>();
            }
            findedElement.iddSUTZ_GUID = record.iddSUTZ_guid;

            // 2.2 установка разделителя учета
            Delimiters delimeter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            findedElement.Delimeter = delimeter;
            hasError += (delimeter != null) ? 0 : 1;

            findedElement.Code = record.Code;
            findedElement.Description = record.Description.Trim();
            //findedElement.TypeOfStockRoom = (TypeOfStockRooms)record.TypeOfStockRoom;
            findedElement.TypeOfStockRoom = (Enum.IsDefined(typeof(TypeOfStockRooms), record.TypeOfStockRoom) ? (TypeOfStockRooms)record.TypeOfStockRoom : TypeOfStockRooms.НеВыбран);
            findedElement.isGroupOfStockRooms = record.isGroup;
            findedElement.LogisticParameter = record.logisticParameter;
            findedElement.RemoveFromMixing = record.removeFromMix;            

            // здесь нужно сделать запись двух объектов в одной транзакции. Как?
            // так же нужно сделать анализ удачности загрузки всех реквизитов элемента. Если хотя бы один не удалось загрузить,
            // пишем в поле "is_read" вместо 1 2,3,4, и т.д до 99. 
            try
            {
                if (recordForRecord != null)
                {
                    recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                    if (hasError > 0)
                    {
                        recordForRecord.is_read = (recordForRecord.is_read == 0) ? (byte)2 : ((recordForRecord.is_read>253)? (byte)0: ((byte)(recordForRecord.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                    }
                    else
                    {
                        recordForRecord.is_read = 1;
                    }
                    
                    recordForRecord.UserHostReader = intCurrentUserHostID;
                    recordForRecord.Save();
                }
                findedElement.Save();
                //objectSpace.CommitChanges();
                delegateForShowMessage(String.Format("Успешно загружена кладовая, наименование = {0}", findedElement.Description));
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        // метод для загрузки всех записей по справочнику кладовых
        private bool LoadAllStockrooms()
        {
            XPQuery<SUTZ_NET_StockRooms> records = new XPQuery<SUTZ_NET_StockRooms>(((XPObjectSpace)objectSpace).Session);
            var listOfUnits = from c in records where c.is_read != 1 select c;
            foreach (SUTZ_NET_StockRooms unit in listOfUnits)
            {
                bool isLoading = loadOneStockRoom(unit);
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
        #endregion

        #region LoadingStorageCodes
        private bool loadOneStorageCode(SUTZ_NET_StorageCodes record)
        {
            int hasError = 0;
            // 1. поиск объекта по iddguid. 
            StorageCodes findedElement = findObjectByGUID<StorageCodes>(record.iddSUTZ_guid);
            SUTZ_NET_StorageCodes recordForRecord = objectSpace.FindObject<SUTZ_NET_StorageCodes>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));

            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<StorageCodes>();
                findedElement.iddSUTZ_GUID = record.iddSUTZ_guid;
            }     
            // установка 
            if (findedElement.objectSpace==null)
            {
                findedElement.objectSpace = objectSpace;
            }

            // 2.2 установка разделителя учета
            Delimiters delimiter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            findedElement.Delimiter = delimiter;
            hasError += (delimiter != null) ? 0 : 1;

            findedElement.Code = record.Code;
            findedElement.IsFolder = record.IsFolder;

            // установка родителя
            StorageCodes StorageCodeFolder = objectSpace.FindObject<StorageCodes>(new BinaryOperator("ParentID", record.ParentID, BinaryOperatorType.Equal), true);
            findedElement.ParentID = StorageCodeFolder;
            //hasError += (StorageCodeFolder != null) ? 0 : 1;

            // установка кладовой ячейки
            StockRooms stockRoom = objectSpace.FindObject<StockRooms>(new BinaryOperator("iddSUTZ_GUID", record.StockRoomID, BinaryOperatorType.Equal), true);
            findedElement.Stockroom = stockRoom;
            hasError += (stockRoom != null) ? 0 : 1;
            
            findedElement.Stillage = record.RackNo;
            findedElement.Floor = record.Floor;
            findedElement.Cell= record.Cell;
            findedElement.SubCell = record.SubCell;
            
            // определение типа объема
            //StorageCodes stCode = new StorageCodes(((XPObjectSpace)objectSpace).Session);
            findedElement.CellType = findedElement.getTypeOfCellBySize(record.Height, record.Width, record.Depth);

            // состояние ячейки - используется, не используется.
            if (record.CellState==1||record.CellState==2)
            {
                findedElement.CellState = (CellStates)record.CellState;
            }
            
            // установка прописанного товара в ячейке:
            Goods good = objectSpace.FindObject<Goods>(new BinaryOperator("iddSUTZ_GUID", record.GoodID, BinaryOperatorType.Equal), true);
            findedElement.Good = good;
            //hasError += (good != null) ? 0 : 1;

            findedElement.MaxItems = record.MaxItems;
            findedElement.CodeOfCell = record.CodeOfCell;

            // здесь нужно сделать запись двух объектов в одной транзакции. Как?
            // так же нужно сделать анализ удачности загрузки всех реквизитов элемента. Если хотя бы один не удалось загрузить,
            // пишем в поле "is_read" вместо 1 2,3,4, и т.д до 99. 

            try
            {
                if (recordForRecord != null)
                {
                    recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                    if (hasError > 0)
                    {
                        recordForRecord.is_read = (recordForRecord.is_read == 0) ? (byte)2 : ((recordForRecord.is_read > 253) ? (byte)0 : ((byte)(recordForRecord.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                    }
                    else
                    {
                        recordForRecord.is_read = 1;
                    }

                    recordForRecord.UserHostReader = intCurrentUserHostID;
                    recordForRecord.Save();
                }
                findedElement.Save();
                
                //objectSpace.CommitChanges();
                delegateForShowMessage(String.Format("Успешно загружена кладовая, наименование = {0}", findedElement.Code));
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllStorageCodes()
        {
            XPQuery<SUTZ_NET_StorageCodes> records = new XPQuery<SUTZ_NET_StorageCodes>(((XPObjectSpace)objectSpace).Session);
            var listOfUnits = from c in records where c.is_read != 1 select c;
            foreach (SUTZ_NET_StorageCodes unit in listOfUnits)
            {
                bool isLoading = loadOneStorageCode(unit);
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
        #endregion

        #region LoadingProperties
        private bool loadOneProperty(SUTZ_NET_Properties record)
        {
            int hasError = 0;
            
            // 1. поиск объекта по iddguid. 
            Properties findedElement = findObjectByGUID<Properties>(record.iddSUTZ_guid);
            SUTZ_NET_Properties recordForRecord = objectSpace.FindObject<SUTZ_NET_Properties>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));

            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Properties>();
                findedElement.iddSUTZ_GUID = record.iddSUTZ_guid;
            }

            // 2.2 установка разделителя учета
            Delimiters delimeter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            findedElement.Delimeter = delimeter;
            hasError += (delimeter != null) ? 0 : 1;

            // установка владельца
            Goods goodParent = objectSpace.FindObject<Goods>(new BinaryOperator("iddSUTZ_GUID", record.ParentID, BinaryOperatorType.Equal), true);

            goodParent.PropertiesCollection.Add(findedElement);

            //findedElement.Parent = goodParent;
            hasError += (goodParent != null) ? 0 : 1;
            
            findedElement.Code = record.Code;
            findedElement.Description = record.Description;

            //DateTime? dtLokal=new DateTime(2001, 1, 1, 0,0,0); 
            //if (record.ProductionDate!=null)
            //{
            //    if (record.ProductionDate.Year>2000)
            //    {
            //        dtLokal = record.ProductionDate;
            //    }
            //}
            findedElement.ProductionDate = record.ProductionDate;

            //dtLokal = new DateTime(2001, 1, 1, 0, 0, 0); 
            //if (record.ExpirationDate != null)
            //{
            //    if (record.ProductionDate.Year < 2000)
            //    {
            //        dtLokal = new DateTime(2001, 1, 1, 0, 0, 0);
            //    }
            //}            
            findedElement.ExpirationDate = record.ExpirationDate;
            findedElement.PackKoeff = record.PackCoeff;  
            findedElement.Comment = record.Comment;
            
            try
            {
                if (recordForRecord != null)
                {
                    recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                    if (hasError > 0)
                    {
                        recordForRecord.is_read = (recordForRecord.is_read == 0) ? (byte)2 : ((recordForRecord.is_read > 253) ? (byte)0 : ((byte)(recordForRecord.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                    }
                    else
                    {
                        recordForRecord.is_read = 1;
                    }

                    recordForRecord.UserHostReader = intCurrentUserHostID;
                    recordForRecord.Save();
                }
                findedElement.Save();
                delegateForShowMessage(String.Format("Успешно загружена характеристика, наименование = {0}", findedElement.Code));
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllProperties()
        {
            XPQuery<SUTZ_NET_Properties> records = new XPQuery<SUTZ_NET_Properties>(((XPObjectSpace)objectSpace).Session);
            var listOfUnits = from c in records where c.is_read != 1 select c;
            foreach (SUTZ_NET_Properties unit in listOfUnits)
            {
                bool isLoading = loadOneProperty(unit);

                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
        #endregion

        #region LoadingCountries
        private bool loadOneCountry(SUTZ_NET_Countries record)
        {
            int hasError = 0;

            // 1. поиск объекта по iddguid. 
            Countries findedElement = findObjectByGUID<Countries>(record.iddSUTZ_guid);
            SUTZ_NET_Countries recordForRecord = objectSpace.FindObject<SUTZ_NET_Countries>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));

            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Countries>();
                findedElement.idGUID = record.iddSUTZ_guid;
            }

            // 2.2 установка разделителя учета
            Delimiters delimeter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            findedElement.Delimeter = delimeter;
            hasError += (delimeter != null) ? 0 : 1;

            findedElement.Code = record.Code;
            findedElement.Description = record.Description;
 
            try
            {
                if (recordForRecord != null)
                {
                    recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                    if (hasError > 0)
                    {
                        recordForRecord.is_read = (recordForRecord.is_read == 0) ? (byte)2 : ((recordForRecord.is_read > 253) ? (byte)0 : ((byte)(recordForRecord.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                    }
                    else
                    {
                        recordForRecord.is_read = 1;
                    }

                    recordForRecord.UserHostReader = intCurrentUserHostID;
                    recordForRecord.Save();
                }
                findedElement.Save();
                //objectSpace.CommitChanges();
                delegateForShowMessage(String.Format("Успешно загружена характеристика, наименование = {0}", findedElement.Code));
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllCountries()
        {
            XPQuery<SUTZ_NET_Countries> records = new XPQuery<SUTZ_NET_Countries>(((XPObjectSpace)objectSpace).Session);
            var listOfUnits = from c in records where c.is_read != 1 select c;
            foreach (SUTZ_NET_Countries unit in listOfUnits)
            {
                bool isLoading = loadOneCountry(unit);
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
        #endregion

        #region LoadingManufacturers
        private bool loadOneManufacture(SUTZ_NET_Manufacturers record)
        {
            int hasError = 0;

            // 1. поиск объекта по iddguid. 
            Manufacturers findedElement = findObjectByGUID<Manufacturers>(record.iddSUTZ_guid);
            SUTZ_NET_Manufacturers recordForRecord = objectSpace.FindObject<SUTZ_NET_Manufacturers>(new BinaryOperator("row_id", record.row_id, BinaryOperatorType.Equal));

            if (findedElement == null)
            {
                findedElement = objectSpace.CreateObject<Manufacturers>();
                findedElement.iddSUTZ_GUID = record.iddSUTZ_guid;
            }

            // 2.2 установка разделителя учета
            Delimiters delimeter = objectSpace.FindObject<Delimiters>(new BinaryOperator("DelimeterID", record.Delimeter, BinaryOperatorType.Equal), true);
            findedElement.Delimeter = delimeter;
            hasError += (delimeter != null) ? 0 : 1;

            findedElement.Code = record.Code;
            findedElement.Description = record.Description;

            try
            {
                if (recordForRecord != null)
                {
                    recordForRecord.dTimeRead = DateTime.Now.ToLocalTime();
                    if (hasError > 0)
                    {
                        recordForRecord.is_read = (recordForRecord.is_read == 0) ? (byte)2 : ((recordForRecord.is_read > 253) ? (byte)0 : ((byte)(recordForRecord.is_read + 1)));// счетчик увеличивается до 253, потом снова начинается с 2.
                    }
                    else
                    {
                        recordForRecord.is_read = 1;
                    }

                    recordForRecord.UserHostReader = intCurrentUserHostID;
                    recordForRecord.Save();
                }
                findedElement.Save();
                //objectSpace.CommitChanges();
                delegateForShowMessage(String.Format("Успешно загружена характеристика, наименование = {0}", findedElement.Code));
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        // метод для загрузки всех записей по справочнику кодов размещения.
        private bool LoadAllManufacturers()
        {
            XPQuery<SUTZ_NET_Manufacturers> records = new XPQuery<SUTZ_NET_Manufacturers>(((XPObjectSpace)objectSpace).Session);
            var listOfUnits = from c in records where c.is_read != 1 select c;
            foreach (SUTZ_NET_Manufacturers unit in listOfUnits)
            {
                bool isLoading = loadOneManufacture(unit);
                if (((ICollection)objectSpace.ModifiedObjects).Count >= 10)
                {
                    objectSpace.CommitChanges();
                }
            }
            return true;
        }
        #endregion


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
