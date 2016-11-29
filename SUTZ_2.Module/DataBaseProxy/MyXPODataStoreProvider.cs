using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace SUTZ_2.Module.DataBaseProxy
{
    public class MyXPODataStoreProvider : IXpoDataStoreProvider
    {
        private XpoDataStoreProxy proxy;
        public MyXPODataStoreProvider()
        {
            proxy = new XpoDataStoreProxy();
        }

        private bool isInitialized;
        public bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
        }
        public void Initialize(XPDictionary dictionary, string mainDBConnectionString, string exchangeDBConnectionString)
        {
            proxy.Initialize(dictionary, mainDBConnectionString, exchangeDBConnectionString);
            isInitialized = true;
        }

        #region Члены IXpoDataStoreProvider

        public string ConnectionString
        {
            get { return ""; }
        }

        public DevExpress.Xpo.DB.IDataStore CreateUpdatingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return proxy;
        }

        public DevExpress.Xpo.DB.IDataStore CreateWorkingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return proxy;
        }

        public IDataStore CreateUpdatingStore(bool allowUpdateSchema, out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return proxy;
        }

        public IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects)
        {
            disposableObjects = null;
            return proxy;
        }

        public XPDictionary XPDictionary
        {
            get { return null; }
        }

        #endregion
    }
}
