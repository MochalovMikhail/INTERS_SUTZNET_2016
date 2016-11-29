using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;


namespace SUTZ_2.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class SUTZ_2WindowsFormsModule : ModuleBase
    {
        public SUTZ_2WindowsFormsModule()
        {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            return ModuleUpdater.EmptyModuleUpdaters;
        }
    }
}
