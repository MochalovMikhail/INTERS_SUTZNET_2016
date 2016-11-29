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
using System.IO;
using System.Text;
using System.Windows.Forms;
using SUTZ_2.Module.BO.References;
using DevExpress.Data.Filtering;
using SUTZ_2;
using SUTZ_2.Module.Editors;
using SUTZ_2.Module;
using SUTZ_2.Module.BO;
using DevExpress.ExpressApp.Xpo;
using System.Drawing;
using NLog;
using SUTZ_2.MobileSUTZ;

namespace SUTZ_2.Win
{
	public partial class SymbolMainFormTemplate2 : MainFormTemplateBase, IDockManagerHolder, ISupportClassicToRibbonTransform
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();

		public override void SetSettings(IModelTemplate modelTemplate)
		{
			base.SetSettings(modelTemplate);
			//navigation.Model = TemplatesHelper.GetNavBarCustomizationNode();
			formStateModelSynchronizerComponent.Model = GetFormStateNode();
			//modelSynchronizationManager.ModelSynchronizableComponents.Add(navigation);
		}
		protected virtual void InitializeImages()
		{
            //barMdiChildrenListItem.Glyph = ImageLoader.Instance.GetImageInfo("Action_WindowList").Image;
            //barMdiChildrenListItem.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_WindowList").Image;
            //barSubItemPanels.Glyph = ImageLoader.Instance.GetImageInfo("Action_Navigation").Image;
            //barSubItemPanels.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_Navigation").Image;
		}

		public SymbolMainFormTemplate2(XafApplication xafApp)
		{
			InitializeComponent();
            //InitializeImages();
			UpdateMdiModeDependentProperties();
			//documentManager.BarAndDockingController = mainBarAndDockingController;
			//documentManager.MenuManager = mainBarManager;

			// объект сохраняется как свойсто главного окна:
			objXafApp= (XafApplication)xafApp;

			if (TemplateCreated != null)
			{
				TemplateCreated(this, EventArgs.Empty);
			}

            IObjectSpace objectSpace = currentSessionSettings.ObjXafApp.CreateObjectSpace();
            LogisticsSettings logSettings = new LogisticsSettings(objectSpace.Session());
            Color backColor = logSettings.getBackColorForMobileForms();
            if (!backColor.IsEmpty)
            {
                this.Appearance.BackColor = backColor;
                this.viewSitePanel.Appearance.BackColor = backColor;
            }

            // 2. инициализация таймера для обновления главной формы:
            winTimerMainForm.Tick += new EventHandler(winTimerMainForm_Tick);
            winTimerMainForm.Interval = 5000;
            
            winTimerMainForm.Start();
            // пока временно отключим таймер формы, чтобы не мешал отладке.
            winTimerMainForm.Stop();
		}

  		//public Bar ClassicStatusBar
		//{
		//    get { return _statusBar; }
		//}
		//public DockPanel DockPanelNavigation
		//{
		//    get { return dockPanelNavigation; }
		//}
		public DockManager DockManager
		{
			get { return mainDockManager; }
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (ModelTemplate != null && !string.IsNullOrEmpty(ModelTemplate.DockManagerSettings))
			{
				MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(ModelTemplate.DockManagerSettings));
				DockManager.RestoreLayoutFromStream(stream);
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			if (ModelTemplate != null)
			{
				MemoryStream stream = new MemoryStream();
				DockManager.SaveLayoutToStream(stream);
				ModelTemplate.DockManagerSettings = Encoding.UTF8.GetString(stream.ToArray());
			}
			base.OnClosing(e);
		}
		protected override void UpdateMdiModeDependentProperties()
		{
			//base.UpdateMdiModeDependentProperties();
			bool isMdi = UIType == UIType.StandardMDI || UIType == UIType.TabbedMDI;
			//viewSitePanel.Visible = !isMdi;
			//Do not replace with ? operator (problems with convertion to VB)
			//if (isMdi)
			//{
			//    barSubItemWindow.Visibility = BarItemVisibility.Always;
			//    barMdiChildrenListItem.Visibility = BarItemVisibility.Always;
			//}
			//else
			//{
			//    barSubItemWindow.Visibility = BarItemVisibility.Never;
			//    barMdiChildrenListItem.Visibility = BarItemVisibility.Never;
			//}
		}
		//private void mainBarManager_Disposed(object sender, System.EventArgs e)
		//{
		//    if (this.mainBarManager != null)
		//    {
		//        this.mainBarManager.Disposed -= new System.EventHandler(mainBarManager_Disposed);
		//    }
		//    modelSynchronizationManager.ModelSynchronizableComponents.Remove(barManager);
		//    this.barManager = null;
		//    this.mainBarManager = null;
		//    this._mainMenuBar = null;
		//    //this._statusBar = null;
		//    //this.standardToolBar = null;
		//    this.barDockControlBottom = null;
		//    this.barDockControlLeft = null;
		//    this.barDockControlRight = null;
		//    this.barDockControlTop = null;
		//}
		public static event EventHandler<EventArgs> TemplateCreated;

		private void SymbolMainFormTemplate2_Load(object sender, EventArgs e)
		{
			//this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			//this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			System.Diagnostics.Debug.WriteLine("Вызов SymbolMainFormTemplate2_Load");
            this.Height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            this.Width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            this.WindowState = FormWindowState.Maximized;
            logger.Trace("Разрешение монитора: высота:{0} ширина:{1}", SystemInformation.PrimaryMonitorSize.Height, SystemInformation.PrimaryMonitorSize.Width);
		 }

		// событие возникает при первом отображении формы
		private void SymbolMainFormTemplate2_Shown(object sender, EventArgs e)
		{
			this.ControlBox = false;
			this.Text = "";
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			System.Diagnostics.Debug.WriteLine("Вызов SymbolMainFormTemplate2_Shown");
            this.refreshLabelsTextOnForm();
		}

		private void buttonExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
		private void OpenDetailView()
		{
			XafApplication app = this.ObjXafApp;
			IObjectSpace os = app.CreateObjectSpace();
			//Find an existing object.
			//StorageCodes obj = os.FindObject<StorageCodes>(CriteriaOperator.Parse("FirstName=?", "My Contact"));
			//Or create a new object.
			//Contact obj = os.CreateObject<Contact>();
			//obj.FirstName = "My Contact";
			//Save the changes if necessary.
			//os.CommitChanges();
			//Configure how our View will be displayed (all parameters except for the CreatedView are optional).
			string listViewId = app.FindListViewId(typeof(StockRooms));
			ShowViewParameters svp = new ShowViewParameters();
			svp.CreatedView = app.CreateListView(listViewId,
				app.CreateCollectionSource(os,typeof(StockRooms),listViewId),true); 
			svp.TargetWindow = TargetWindow.NewModalWindow;
			svp.Context = TemplateContext.PopupWindow;
			//svp.CreateAllControllers = true;
			//You can pass custom Controllers for intercommunication or to provide a standard functionality (e.g., functionality of a dialog window).
			//DialogController dc = Application.CreateController<DialogController>();
			//svp.Controllers.Add(dc);
			// Show our View once the ShowViewParameters object is initialized.
			app.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
		}
        private JobTypes showCustomFormSelectJobType()
        {
            Form form =  new WinCustomForm(enVariantsOfSelectedForm.selectJobTypes);
            // Initializing a form when it is invoked from a controller.
            //Form form = DevExpress.Persistent.Base.ReflectionHelper.CreateObject("ytcn") as Form;
            XpoSessionAwareControlInitializer.Initialize(form as IXpoSessionAwareControl, ObjXafApp);
            form.Owner = this;
            form.Show();
            
            //DialogResult result= form.ShowDialog(this);
            return null;

        }
		private void buttonSelectJobType_Click(object sender, EventArgs e)
		{
			//OpenDetailView();
            JobTypes currentJobType = showCustomFormSelectJobType();
		}
        // метод вызывается из дочерней формы или из других мест
        public void onSuccesSelectJobType(JobTypes jobType)
        {
            currentSessionSettings.CurrentJobType = jobType;
            this.refreshLabelsTextOnForm();
        }

        private void buttonBeginWork_Click(object sender, EventArgs e)
        {
            XPObjectSpace objSpace = (DevExpress.ExpressApp.Xpo.XPObjectSpace)ObjXafApp.CreateObjectSpace();
            MobileSUTZ_main mobileClass = new MobileSUTZ_main(objSpace.Session);
            mobileClass.runWorkBySelectedWorkType();
        }

        private void SymbolMainFormTemplate2_Activated(object sender, EventArgs e)
        {
            //XPObjectSpace objectSpace = (XPObjectSpace)currentSessionSettings.ObjXafApp.CreateObjectSpace();
            //LogisticsSettings logSettings = new LogisticsSettings(objectSpace.Session);
            //Color backColor = logSettings.getBackColorForMobileForms();
            //if (!backColor.IsEmpty)
            //{
            //    this.Appearance.BackColor = backColor;
            //}
        }
      }
}
