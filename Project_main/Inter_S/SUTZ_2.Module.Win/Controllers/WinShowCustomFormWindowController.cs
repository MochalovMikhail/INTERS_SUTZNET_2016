using System;
using SUTZ_2.Module.Editors;
using System.Windows.Forms;
using System.ComponentModel;
using SUTZ_2.Module.Controllers;
using DevExpress.ExpressApp.SystemModule;


namespace SUTZ_2.Module.Win.Controllers {
    /// <summary>
    /// A custom Application Model element extension for the Navigation Item node to be able to specify custom Win Forms via the Model Editor.
    /// </summary>
    public interface IModelWinCustomFormPathNavigationItem {
        [Category("Data")]
        string CustomFormTypeName { get; set; }
    }
    public class WinShowCustomFormWindowController : ShowCustomFormWindowController {
        protected override void ShowCustomForm(IModelNavigationItem model) {
            string customFormTypeName = ((IModelWinCustomFormPathNavigationItem)model).CustomFormTypeName;
            Form form = DevExpress.Persistent.Base.ReflectionHelper.CreateObject(customFormTypeName) as Form;
            // Initializing a form when it is invoked from a controller.
            
            XpoSessionAwareControlInitializer.Initialize(form as IXpoSessionAwareControl, Application);

            form.Show();
        }
    }
}