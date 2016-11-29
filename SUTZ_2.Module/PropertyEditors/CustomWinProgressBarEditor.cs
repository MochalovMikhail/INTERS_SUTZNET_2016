using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;
using SUTZ_2.Module.Controls;

namespace SUTZ_2.Module.PropertyEditors
{
    [PropertyEditor(typeof(int), CustomWinProgressBarEditor.WinProgressBarEditorAlias, false)]
    public class CustomWinProgressBarEditor : DXPropertyEditor
    {
        public const string WinProgressBarEditorAlias = "WinProgressBarEditor";

        public CustomWinProgressBarEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) 
        {
            this.ControlBindingProperty = "EditValue";
        }

        protected override object CreateControlCore() 
        {
            return new CustomProgressBarControl();
        }
        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemCustomProgressBarControl();
        }
        protected override void SetupRepositoryItem(RepositoryItem item) {
            RepositoryItemCustomProgressBarControl repositoryItem = (RepositoryItemCustomProgressBarControl)item;
            repositoryItem.Maximum = 100;
            repositoryItem.Minimum = 0;
            repositoryItem.PercentView = false;
            repositoryItem.Step = 1;
            base.SetupRepositoryItem(item);
        }
    }
}
