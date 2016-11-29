using DevExpress.XtraBars;
using DevExpress.ExpressApp.Win.Templates;

namespace SUTZ_2.Win
{
    partial class SymbolMainFormTemplate2
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolMainFormTemplate2));
            this.mainBarAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.mainDockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.barSubItemPanels = new DevExpress.XtraBars.BarSubItem();
            this.Window = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
            this.labelControlJobType = new DevExpress.XtraEditors.LabelControl();
            this.buttonBeginWork = new DevExpress.XtraEditors.SimpleButton();
            this.buttonSelectJobType = new DevExpress.XtraEditors.SimpleButton();
            this.buttonExit = new DevExpress.XtraEditors.SimpleButton();
            this.formStateModelSynchronizerComponent = new DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.documentManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainBarAndDockingController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainDockManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
            this.viewSitePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelSynchronizationManager
            // 
            this.modelSynchronizationManager.ModelSynchronizableComponents.Add(this.formStateModelSynchronizerComponent);
            // 
            // viewSiteManager
            // 
            this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
            // 
            // mainBarAndDockingController
            // 
            this.mainBarAndDockingController.PropertiesBar.AllowLinkLighting = false;
            this.mainBarAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
            this.mainBarAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
            // 
            // mainDockManager
            // 
            this.mainDockManager.Controller = this.mainBarAndDockingController;
            this.mainDockManager.Form = this;
            this.mainDockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.ExpressApp.Win.Templates.Controls.XafRibbonControl",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar"});
            // 
            // barSubItemPanels
            // 
            this.barSubItemPanels.Id = -1;
            this.barSubItemPanels.Name = "barSubItemPanels";
            // 
            // Window
            // 
            resources.ApplyResources(this.Window, "Window");
            this.Window.ContainerId = "Windows";
            this.Window.Id = 34;
            this.Window.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
            this.Window.Name = "Window";
            this.Window.TargetPageCategoryColor = System.Drawing.Color.Empty;
            // 
            // viewSitePanel
            // 
            resources.ApplyResources(this.viewSitePanel, "viewSitePanel");
            this.viewSitePanel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("viewSitePanel.Appearance.BackColor")));
            this.viewSitePanel.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("viewSitePanel.Appearance.ForeColor")));
            this.viewSitePanel.Appearance.Options.UseBackColor = true;
            this.viewSitePanel.Appearance.Options.UseForeColor = true;
            this.viewSitePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewSitePanel.Controls.Add(this.labelControlJobType);
            this.viewSitePanel.Controls.Add(this.buttonBeginWork);
            this.viewSitePanel.Controls.Add(this.buttonSelectJobType);
            this.viewSitePanel.Controls.Add(this.buttonExit);
            this.viewSitePanel.Name = "viewSitePanel";
            // 
            // labelControlJobType
            // 
            resources.ApplyResources(this.labelControlJobType, "labelControlJobType");
            this.labelControlJobType.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("labelControlJobType.Appearance.BackColor")));
            this.labelControlJobType.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControlJobType.Appearance.Font")));
            this.labelControlJobType.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControlJobType.Appearance.ForeColor")));
            this.labelControlJobType.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.labelControlJobType.Name = "labelControlJobType";
            // 
            // buttonBeginWork
            // 
            resources.ApplyResources(this.buttonBeginWork, "buttonBeginWork");
            this.buttonBeginWork.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("buttonBeginWork.Appearance.Font")));
            this.buttonBeginWork.Appearance.Options.UseFont = true;
            this.buttonBeginWork.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.buttonBeginWork.Name = "buttonBeginWork";
            this.buttonBeginWork.Click += new System.EventHandler(this.buttonBeginWork_Click);
            // 
            // buttonSelectJobType
            // 
            resources.ApplyResources(this.buttonSelectJobType, "buttonSelectJobType");
            this.buttonSelectJobType.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("buttonSelectJobType.Appearance.Font")));
            this.buttonSelectJobType.Appearance.Options.UseFont = true;
            this.buttonSelectJobType.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.buttonSelectJobType.Name = "buttonSelectJobType";
            this.buttonSelectJobType.Click += new System.EventHandler(this.buttonSelectJobType_Click);
            // 
            // buttonExit
            // 
            resources.ApplyResources(this.buttonExit, "buttonExit");
            this.buttonExit.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("buttonExit.Appearance.Font")));
            this.buttonExit.Appearance.Options.UseFont = true;
            this.buttonExit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // formStateModelSynchronizerComponent
            // 
            this.formStateModelSynchronizerComponent.Form = this;
            this.formStateModelSynchronizerComponent.Model = null;
            // 
            // SymbolMainFormTemplate2
            // 
            this.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("SymbolMainFormTemplate2.Appearance.BackColor")));
            this.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("SymbolMainFormTemplate2.Appearance.ForeColor")));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.viewSitePanel);
            this.IsMdiContainer = true;
            this.Name = "SymbolMainFormTemplate2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.SymbolMainFormTemplate2_Activated);
            this.Load += new System.EventHandler(this.SymbolMainFormTemplate2_Load);
            this.Shown += new System.EventHandler(this.SymbolMainFormTemplate2_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.documentManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainBarAndDockingController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainDockManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
            this.viewSitePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        //private DevExpress.XtraBars.BarDockControl barDockControlTop;
        //private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        //private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        //private DevExpress.XtraBars.BarDockControl barDockControlRight;
        //private DevExpress.ExpressApp.Win.Templates.Controls.XafBar _mainMenuBar;
        //private DevExpress.ExpressApp.Win.Templates.Controls.XafBar standardToolBar;
        // private DevExpress.ExpressApp.Win.Templates.Controls.XafBar _statusBar;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFile;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cObjectsCreation;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cPrint;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExport;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cSave;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cUndoRedo;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cAppearance;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cReports;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cEdit;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExit;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cOpenObject;
        //private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemFile;
        //private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemEdit;
        //private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemView;
        //private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemTools;
        //private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemHelp;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cViewsNavigation;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordEdit;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cWorkflow;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordsNavigation;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cViewsHistoryNavigation;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cSearch;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFullTextSearch;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFilters;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cView;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cDefault;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cTools;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cDiagnostic;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cOptions;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cAbout;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.XafBarLinkContainerItem cWindows;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cPanels;
        //private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cMenu;
        //private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemWindow;
        //private DevExpress.XtraBars.BarDockingMenuItem barMdiChildrenListItem;

        #endregion

        protected DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer formStateModelSynchronizerComponent;
        private BarAndDockingController mainBarAndDockingController;
        private DevExpress.XtraBars.Docking.DockManager mainDockManager;
        //protected DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager mainBarManager;
        private DevExpress.XtraEditors.PanelControl viewSitePanel;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem Window;
        private BarSubItem barSubItemPanels;
        private DevExpress.XtraEditors.SimpleButton buttonExit;
        private DevExpress.XtraEditors.SimpleButton buttonSelectJobType;
        private DevExpress.XtraEditors.LabelControl labelControlJobType;
        private DevExpress.XtraEditors.SimpleButton buttonBeginWork;
    }
}
