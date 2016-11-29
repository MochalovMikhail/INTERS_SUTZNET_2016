using System;
using System.Collections.Generic;
using SUTZ_2.Module.BO.References;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET;


namespace SUTZ_2.Module
{
    public sealed partial class SUTZ_2Module : ModuleBase
    {
        public SUTZ_2Module()
        {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application)
        {
            //application.CreateCustomLogonWindowObjectSpace += new EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs>(application_CreateCustomLogonWindowObjectSpace);
            base.Setup(application);
            application.CustomProcessShortcut+=new EventHandler<CustomProcessShortcutEventArgs>(application_CustomProcessShortcut);
        }

        void application_CustomProcessShortcut(object sender, CustomProcessShortcutEventArgs e)
        {
            if (e.Shortcut.ViewId == "SQL_Exchange_Inbound_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                SQL_Exchange_Inbound sqlExchangeInbound = new SQL_Exchange_Inbound();
                e.View = Application.CreateDetailView(objectSpace, sqlExchangeInbound, true);
                //e.View.AllowEdit["CanEditIssueStatistics"] = false;
                e.Handled = true;
            }
        }
        void application_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e)
        {
            //IObjectSpace objectSpace = ((XafApplication)sender).CreateObjectSpace();
            //((CustomLogonParametersUsers)e.LogonParameters).ObjectSpace = objectSpace;
        }
    }
}
