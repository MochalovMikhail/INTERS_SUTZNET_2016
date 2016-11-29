using System;
using System.Linq;
using System.Collections.Generic;

namespace SUTZ_2.Module.Win {
    public sealed partial class SUTZ_2WindowsFormsModule
    {
        // Extends the Application Model elements for View and Navigation Items to be able to specify custom controls via the Model Editor.
        // Refer to the http://documentation.devexpress.com/#Xaf/CustomDocument3169 help article for more information.
        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<SUTZ_2.Module.Editors.IModelCustomUserControlViewItem, SUTZ_2.Module.Win.Editors.IModelWinCustomUserControlViewItem>();
            extenders.Add<DevExpress.ExpressApp.SystemModule.IModelNavigationItem, SUTZ_2.Module.Win.Controllers.IModelWinCustomFormPathNavigationItem>();
        }
    }
}
