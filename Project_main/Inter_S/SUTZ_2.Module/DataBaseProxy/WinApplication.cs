using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using NLog;

namespace SUTZ_2.Module.DataBaseProxy
{
    public class XpoDataStoreProxy : IDataStore
    {
        // переменные для подключения к основной БД
        #region ЛокальныеДобавленныеПеременные
        private SimpleDataLayer mainDBDataLayer;
        private IDataStore mainDBDataStore;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // переменные для подключения к БД обмена с СУТЗ 1С:7.7
        private SimpleDataLayer exchangeDB1CDataLayer;
        private IDataStore exchangeDB1CDataStore;
        // список таблиц в базе данных обмена
        private string[] exchangeDB1CDatabaseTables = new string[] { 
            "SUTZ_NET_Clients",
            "SUTZ_NET_Countries",
            "SUTZ_NET_CustomerStores",
            "SUTZ_NET_CustomerUnits",
            "SUTZ_NET_DocTrebovanieHeaders",
            "SUTZ_NET_DocTrebovanieGoods",
            "SUTZ_NET_DocInventoryGoods",
            "SUTZ_NET_DocInventoryHeaders",
            "SUTZ_NET_DocInvoiceOrderHeaders",
            "SUTZ_NET_DocInvoiceOrderGoods",
            "SUTZ_NET_DocSpecPeremeshHeaders",
            "SUTZ_NET_DocSpecPeremeshGoods",
            "SUTZ_NET_DocSpecPrihodaHeaders",
            "SUTZ_NET_DocSpecPrihodaGoods",
            "SUTZ_NET_DocSpecRashodaHeaders",
            "SUTZ_NET_DocSpecRashodaGoods",
            "SUTZ_NET_DocSpecTrebovanieHeaders",
            "SUTZ_NET_DocSpecTrebovanieGoods",
            "SUTZ_NET_Goods", 
            "SUTZ_NET_HostsUsers",
            "SUTZ_NET_JobTypes",
            "SUTZ_NET_Logistics",
            "SUTZ_NET_Manufacturers",
            "SUTZ_NET_OKEI",
            "SUTZ_NET_Properties",
            "SUTZ_NET_StockRooms",
            "SUTZ_NET_StorageCodes",
            "SUTZ_NET_Units",
            "SUTZ_NET_Warehouses",
            "SUTZ_NET_BarcodesOfGoods",
            "SUTZ_NET_OperationState",
            "NET_SUTZ_DocSpecPrihodaHeaders",
            "NET_SUTZ_DocSpecPrihodaGoods",
            "NET_SUTZ_DocSpecPeremeshHeaders",
            "NET_SUTZ_DocSpecPeremeshGoods",
            "NET_SUTZ_MobileWorkUnit",
            "NET_SUTZ_MobileWorkDocRows",
            "NET_SUTZ_MobileWorkUsers",
            "SUTZ_NET_DocProperties",
            "SUTZ_NET_Users",
            "SUTZ_NET_DocJobeProperties",
            "NET_SUTZ_DocJobeProperties",
            "NET_SUTZ_DocSpecRashHeaders",
            "NET_SUTZ_DocSpecRashGoods",
            };

        // метод определяет по имени таблицы, что она относится к таблицам БД обмена с СУТЗ 1С:7.7
        private bool IsExchangeDatabaseTable(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                foreach (string currentTableName in exchangeDB1CDatabaseTables)
                {
                    if (tableName.EndsWith(currentTableName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public XpoDataStoreProxy()
        {
        }
        public void Initialize(XPDictionary dictionary, string mainDBConnectionString, string exchangeDBConnectionString)
        {
            ReflectionDictionary mainDBDictionary = new ReflectionDictionary();
            ReflectionDictionary exchangeDBDictionary = new ReflectionDictionary();
            foreach (XPClassInfo ci in dictionary.Classes)
            {
                if (IsExchangeDatabaseTable(ci.TableName))
                {
                    exchangeDBDictionary.QueryClassInfo(ci.ClassType);
                }
                else
                {
                    mainDBDictionary.QueryClassInfo(ci.ClassType);
                }
            }
            mainDBDataStore = XpoDefault.GetConnectionProvider(mainDBConnectionString, AutoCreateOption.DatabaseAndSchema);
            mainDBDataLayer = new SimpleDataLayer(mainDBDictionary, mainDBDataStore);

            exchangeDB1CDataStore = XpoDefault.GetConnectionProvider(exchangeDBConnectionString, AutoCreateOption.DatabaseAndSchema);
            exchangeDB1CDataLayer = new SimpleDataLayer(exchangeDBDictionary, exchangeDB1CDataStore);
        }

        #endregion

        #region Члены IDataStore

        public AutoCreateOption AutoCreateOption
        {
            get { return AutoCreateOption.DatabaseAndSchema; }
        }

        // Updates data in a data store using the specified modification statements.
        public ModificationResult ModifyData(params ModificationStatement[] dmlStatements)
        {
            List<ModificationStatement> mainDBChanges = new List<ModificationStatement>(dmlStatements.Length);
            List<ModificationStatement> exchangeDBChanges = new List<ModificationStatement>(dmlStatements.Length);

            foreach (ModificationStatement stm in dmlStatements)
            {
                if (IsExchangeDatabaseTable(stm.Table.Name))
                {
                    exchangeDBChanges.Add(stm);
                }
                else
                {
                    mainDBChanges.Add(stm);
                }
            }

            List<ParameterValue> resultSet = new List<ParameterValue>();
            if (mainDBChanges.Count > 0)
            {
                resultSet.AddRange(mainDBDataLayer.ModifyData(mainDBChanges.ToArray()).Identities);
            }
            if (exchangeDBChanges.Count > 0)
            {
                resultSet.AddRange(exchangeDB1CDataLayer.ModifyData(exchangeDBChanges.ToArray()).Identities);
            }
            return new ModificationResult(resultSet);
        }

        //When implemented by a class, fetches data from a data store using the specified query statements.
        public SelectedData SelectData(params SelectStatement[] selects)
        {
            List<SelectStatement> mainDBSelects = new List<SelectStatement>(selects.Length);
            List<SelectStatement> exchangeDBSelects = new List<SelectStatement>(selects.Length);

            foreach (SelectStatement stm in selects)
            {
                if (IsExchangeDatabaseTable(stm.Table.Name))
                {
                    exchangeDBSelects.Add(stm);
                }
                else
                {
                    mainDBSelects.Add(stm);
                }
            }

            List<SelectStatementResult> resultSet = new List<SelectStatementResult>();
            if (mainDBSelects.Count > 0)
            {
                resultSet.AddRange(mainDBDataLayer.SelectData(mainDBSelects.ToArray()).ResultSet);
            }
            if (exchangeDBSelects.Count > 0)
            {
                resultSet.AddRange(exchangeDB1CDataLayer.SelectData(exchangeDBSelects.ToArray()).ResultSet);
            }
            return new SelectedData(resultSet.ToArray());
        }

        //When implemented by a class, updates the storage schema according to the specified class descriptions.
        public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            List<DBTable> db1Tables = new List<DBTable>();
            List<DBTable> db2Tables = new List<DBTable>();

            foreach (DBTable table in tables)
            {
                if (!IsExchangeDatabaseTable(table.Name))
                {
                    db1Tables.Add(table);
                }
                else
                {
                    db2Tables.Add(table);
                }
            }
            //logger.Debug("db1Tables = "+db1Tables);
            //logger.Debug("db2Tables = " + db1Tables);
            //System.Windows.Forms.MessageBox.Show("db1Tables=" + db1Tables.ToArray().Count() + ", dontCreateIfFirstTableNotExist=" + dontCreateIfFirstTableNotExist, "");

            mainDBDataStore.UpdateSchema(false, db1Tables.ToArray());
            exchangeDB1CDataStore.UpdateSchema(false, db2Tables.ToArray());

            return UpdateSchemaResult.SchemaExists;
        }
        #endregion
    }
}
