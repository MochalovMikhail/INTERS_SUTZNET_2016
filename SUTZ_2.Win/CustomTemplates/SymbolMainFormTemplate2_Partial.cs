using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SUTZ_2.Module.BO.References;
using DevExpress.Data.Filtering;
using SUTZ_2;
using SUTZ_2.Module.Editors;
using SUTZ_2.Module;

namespace SUTZ_2.Win
{
    public partial class SymbolMainFormTemplate2 : MainFormTemplateBase, IDockManagerHolder, ISupportClassicToRibbonTransform
    {
        private XafApplication objXafApp;
        private static System.Windows.Forms.Timer winTimerMainForm = new System.Windows.Forms.Timer();


        public XafApplication ObjXafApp
        {
            get { return objXafApp; }// свойство доступно только для чтения;
        }

        public void refreshLabelsTextOnForm()
        {
            if (currentSessionSettings.CurrentJobType!=null)
            {
                this.labelControlJobType.Text = "Вид работы: " + currentSessionSettings.CurrentJobType.Description;
            }
        }

        // обработчик таймера обновления главного окна:
        void winTimerMainForm_Tick(object sender, EventArgs e)
        {
            refreshLabelsTextOnForm();
        }

        //private void InitializeComponent()
        //{
        //    ((System.ComponentModel.ISupportInitialize)(this.documentManager)).BeginInit();
        //    this.SuspendLayout();
        //    // 
        //    // SymbolMainFormTemplate2
        //    // 
        //    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        //    this.ClientSize = new System.Drawing.Size(284, 262);
        //    this.Name = "SymbolMainFormTemplate2";
        //    this.Text = "";
        //    ((System.ComponentModel.ISupportInitialize)(this.documentManager)).EndInit();
        //    this.ResumeLayout(false);

        //}

    }
}
