using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using SUTZ_2.Module.DataBaseProxy;
using System.Configuration;

namespace SUTZ_2.Win
{
    public partial class SUTZ_2WindowsFormsApplication : WinApplication
    {
        public SUTZ_2WindowsFormsApplication()
        {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            //this.IsAsyncServerMode = true;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
        private void SUTZ_2WindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e)
        {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
                e.Updater.Update();
                e.Handled = true;
            //}
            //else
            //{
            //    throw new InvalidOperationException(
            //        "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
            //        "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
            //        "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
            //        "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
            //        "or manually create a database using the 'DBUpdater' tool.\r\n" +
            //        "Anyway, refer to the 'Update Application and Database Versions' help topic at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm " +
            //        "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/");
            //}
#endif
        }
        private void SUTZ_2WindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e)
        {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1)
            {
                e.Languages.Add(userLanguageName);
            }
        }
        private static MyXPODataStoreProvider provider;
        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            base.OnCreateCustomObjectSpaceProvider(args);
            provider = new MyXPODataStoreProvider();
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(provider);
        }

        protected override void OnCustomCheckCompatibility(CustomCheckCompatibilityEventArgs args)
        {
            base.OnCustomCheckCompatibility(args);
            if (!provider.IsInitialized)
            {
                provider.Initialize(((XPObjectSpaceProvider)this.ObjectSpaceProvider).XPDictionary,
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString,
                    ConfigurationManager.ConnectionStrings["ConnectionStringExchange1C_DB"].ConnectionString);
            }
        }


    }
}
