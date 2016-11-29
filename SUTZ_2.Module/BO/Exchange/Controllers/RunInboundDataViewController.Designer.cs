namespace SUTZ_2.Module.BO.Exchange.Controllers
{
    partial class RunInboundDataViewController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DoRunInboundData = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // DoRunInboundData
            // 
            this.DoRunInboundData.Caption = "¬ключить обмен";
            this.DoRunInboundData.ConfirmationMessage = null;
            this.DoRunInboundData.Id = "DoRunInboundData";
            this.DoRunInboundData.ToolTip = null;
            this.DoRunInboundData.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DoRunInboundData_Execute);
            // 
            // RunInboundDataViewController
            // 
            this.TargetObjectType = typeof(SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET.SQL_Exchange_Inbound);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.ViewControlsCreated += new System.EventHandler(this.RunInboundDataViewController_ViewControlsCreated);
            this.Activated += new System.EventHandler(this.RunInboundDataViewController_Activated);
            this.Deactivated += new System.EventHandler(this.RunInboundDataViewController_Deactivated);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction DoRunInboundData;
    }
}
