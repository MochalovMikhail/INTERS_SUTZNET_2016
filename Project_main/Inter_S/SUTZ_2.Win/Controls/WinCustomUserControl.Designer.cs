// Developer Express Code Central Example:
// How to show custom forms and controls in XAF (Example)
// 
// This example implements the following scenarios when an end-user clicks on a
// custom item in the navigation control:
// - a custom non-XAF form is opened as a
// result;
// - a standard XAF View containing a custom user control is opened as a
// result.
// Both custom form and user controls display persistent data from the XAF
// application database:
// 
// 
// 
// To accomplish this, you can follow the instructions
// below:
// 
// 1. Define a base structure of the navigation control
// (http://documentation.devexpress.com/#Xaf/CustomDocument3198) in your XAF
// application, as shown in the E911.Module\Model.DesignedDiffs.xafml file.
// You
// can simply copy and paste the contents of the NavigationItems element into the
// corresponding file (pre-opened via the text editor) of your platform-agnostic
// module.
// The same customizations can be achieved via the Model Editor
// visually.
// 
// 
// This navigation structure will be further customized in the
// WinForms and ASP.NET executable projects later.
// 
// 2. Intercept events of the
// navigation control to display a custom form when a custom navigation item is
// clicked.
// To do this, implement a WindowController into your platform-agnostic
// module and handle events of the ShowNavigationItemController
// (http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppSystemModuleShowNavigationItemControllertopic)
// class as per the E911.Module\Controllers\ShowCustomFormWindowController.xx file.
// This controller will be abstract and be overridden in WinForms and ASP.NET
// modules.
// 
// 3. Declare a custom ViewItem class
// (http://documentation.devexpress.com/#Xaf/CustomDocument2695) that is supposed
// to host a custom user control in the standard XAF View. To do this, implement a
// custom ViewItem descendant and related types in the platform-agnostic module as
// shown in the E911.Module\Editors\CustomUserControlViewItem.xx file. This
// ViewItem will also be abstract and platform-agnostic as it will not create
// platform-dependent controls, and will just provide a common customization code
// for both platforms. For instance, the OnControlCreated method will be overridden
// to bind the created control to data. To access persistent data from the database
// used by an XAF application, the ViewItem will implement the IComplexViewItem
// interface that consists of a single Setup method, receiving the IObjectSpace and
// XafApplication objects as parameters. To unify our data binding code for both
// platforms, the IXpoSessionAwareControl interface and an auxiliary
// XpoSessionAwareControlInitializer class are introduced.
// The interface provides
// a single UpdateDataSource method that is implemented by custom forms and user
// controls to bind them to data received by means of XPO.
// You can use a similar
// mechanism and modify these auxiliary types to pass other custom data into your
// custom forms and controls.
// 
// 4. Define a base structure of the standard XAF
// View (http://documentation.devexpress.com/#Xaf/CustomDocument2612) with a custom
// ViewItem as shown in the E911.Module\Model.DesignedDiffs.xafml file.
// You can
// simply copy and paste the contents of the Views element into the corresponding
// file (pre-opened via the text editor) of your platform-agnostic
// module.
// 
// 
// 
// 5. Create custom forms and user controls in WinForms and ASP.NET
// executable projects analogous with what is shown in the example. The easiest way
// to do this is to copy the contents of the E911.Win\Controls and
// E911.Web\Controls folders and then include the necessary files into your
// solution. Take special note that these custom forms and controls implement the
// IXpoSessionAwareControl interface to automatically receive persistent data and
// other parameters when they are created.
// 
// 
// 
// 6. Implement platform-dependent
// behavior to open and customize custom forms and controls. To do this, copy the
// following files into your WinForms and ASP.NET module
// projects:
// WinForms:
// E911.Module.Win\Controllers\WinShowCustomFormWindowController.xx
// - contains an WinForms version of the WindowController, which is inherited from
// the platform-agnostic E911.Module.Win\Editors\WinCustomUserControlViewItem.xx -
// contains an WinForms version of the ViewItem, which is inherited from the
// platform-agnostic one;
// E911.Module.Win\WinModuleEx.xx - contains a registration
// code for WinForms version of the
// ViewItem;
// one;
// 
// ASP.NET:
// E911.Module.Web\Editors\WebCustomUserControlViewItem.xx
// - contains an ASP.NET version of the ViewItem, which is inherited from the
// platform-agnostic one;
// E911.Module.Web\WebModuleEx.xx - contains a registration
// code for ASP.NET version of the
// ViewItem;
// E911.Module.Web\Controllers\WebShowCustomFormWindowController.xx -
// contains an ASP.NET version of the WindowController, which is inherited from the
// platform-agnostic one;
// 
// These platform-dependent versions of the
// WindowController and ViewItem are required to implement the creation and display
// of custom forms and controls using the means specific for each platform. They
// are also designed to provide the capability to be able to set custom forms and
// control settings via the Model Editor. For that purpose, custom Application
// Model extensions are implemented for the Navigation Item and View Item model
// elements.
// 
// 7. Set the custom forms and controls settings for each platform. To
// do this, copy the contents of the E911.Win\Model.xafml and E911.Web\Model.xafml
// files into the Model.xafml file in the executable WinForms and ASP.NET
// projects:
// 
// WinForms:
// 
// 
// ASP.NET:
// 
// 
// 
// That is it. IMPORTANT NOTES
// 1. It
// is also possible to mix the traditional and XAF development approaches (consult
// our Support Team if you are not sure how to integrate your standard non-XAF
// solution into XAF), because an XAF application is a regular .NET application
// built of reusable blocks like View, ViewItem, Property and List Editors, etc.
// (http://documentation.devexpress.com/#Xaf/CustomDocument2638) that eventually
// create and customize platform-dependent controls exactly the same way you do
// this without XAF. So, using XAF does not mean something absolutely new and
// allows you to reuse your existing development skills and practices. Of course,
// it is possible to create your own reusable blocks if the standard ones do not
// meet your needs. For instance, the example of a custom View class designed to
// show a custom form can be found on CodeProject here
// (http://www.codeproject.com/Tips/464188/How-to-Show-Usual-Winform-as-View-in-XAF).
// 
// 2.
// This solution contains some generic code (e.g., base WindowController and
// ViewItem) that is mainly required because our XAF application is for both
// Windows and the Web. You may avoid this generic code and make a simpler
// implementation if you are developing for only one platform.
// 
// 3. You can
// display custom forms not only when interacting with the navigation control, but
// from any other place. To do this, intercept the events of a required entity,
// e.g., an XAF Controller, Action or a View. Refer to the product documentation or
// consult with our Support Team in case of any difficulties.
// 
// 4. By default
// controls layout and user customizations are preserved only for built-in XAF
// ListEditors, because they have special code for that. If you embed a custom user
// control into XAF, you need to preserve its settings yourself as well, exactly
// like you would do when implementing this task in the "old way" in a non-XAF
// application. Refer to the control's documentation to learn more on how to
// accomplish this task.
// Feel free to contact the respective product team if you
// experience any difficulties customizing this control.
// 
// See
// also:
// http://www.devexpress.com/scid=K18117
// http://www.devexpress.com/scid=KA18606
// http://www.devexpress.com/scid=K18118
// http://www.devexpress.com/scid=E244
// http://www.devexpress.com/scid=E980
// How
// to: Display a Non-Persistent Object's Detail View from the Navigation
// (http://documentation.devexpress.com/#Xaf/CustomDocument3471)
// ShowNavigationItemController.CustomShowNavigationItem
// Event
// (ms-help://DevExpress.Xaf/DevExpressExpressAppSystemModuleShowNavigationItemController_CustomShowNavigationItemtopic.htm)
// XafApplication.CustomProcessShortcut
// Event
// (ms-help://DevExpress.Xaf/DevExpressExpressAppXafApplication_CustomProcessShortcuttopic.htm)
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E911

namespace SUTZ_2.Win
{
    partial class WinCustomUserControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridViewSelectJobType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnPgDown = new DevExpress.XtraEditors.SimpleButton();
            this.btnPageDown = new DevExpress.XtraEditors.SimpleButton();
            this.btnUp = new DevExpress.XtraEditors.SimpleButton();
            this.btnDn = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectJobType)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            gridLevelNode2.RelationName = "Level1";
            this.gridControl1.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode2});
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridViewSelectJobType;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(0);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.ShowOnlyPredefinedDetails = true;
            this.gridControl1.Size = new System.Drawing.Size(480, 576);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSelectJobType});
            // 
            // gridViewSelectJobType
            // 
            this.gridViewSelectJobType.Appearance.FocusedRow.BackColor = System.Drawing.Color.Red;
            this.gridViewSelectJobType.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewSelectJobType.Appearance.SelectedRow.BackColor = System.Drawing.Color.DarkOrange;
            this.gridViewSelectJobType.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewSelectJobType.GridControl = this.gridControl1;
            this.gridViewSelectJobType.Name = "gridViewSelectJobType";
            this.gridViewSelectJobType.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewSelectJobType.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewSelectJobType.OptionsBehavior.AllowIncrementalSearch = true;
            this.gridViewSelectJobType.OptionsBehavior.Editable = false;
            this.gridViewSelectJobType.OptionsBehavior.ReadOnly = true;
            this.gridViewSelectJobType.OptionsView.ShowGroupPanel = false;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnPgDown.Location = new System.Drawing.Point(111, 582);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(67, 55);
            this.btnPgDown.TabIndex = 5;
            this.btnPgDown.Text = "PgUp";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // btnPageDown
            // 
            this.btnPageDown.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPageDown.Appearance.Options.UseFont = true;
            this.btnPageDown.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnPageDown.Location = new System.Drawing.Point(369, 582);
            this.btnPageDown.Name = "btnPageDown";
            this.btnPageDown.Size = new System.Drawing.Size(80, 54);
            this.btnPageDown.TabIndex = 6;
            this.btnPageDown.Text = "PgDn";
            this.btnPageDown.Click += new System.EventHandler(this.btnPageDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnUp.Location = new System.Drawing.Point(27, 582);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(74, 55);
            this.btnUp.TabIndex = 7;
            this.btnUp.Tag = "";
            this.btnUp.Text = "Up";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDn
            // 
            this.btnDn.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDn.Appearance.Options.UseFont = true;
            this.btnDn.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.btnDn.Location = new System.Drawing.Point(270, 582);
            this.btnDn.Name = "btnDn";
            this.btnDn.Size = new System.Drawing.Size(82, 54);
            this.btnDn.TabIndex = 8;
            this.btnDn.Text = "Dn";
            this.btnDn.Click += new System.EventHandler(this.btnDn_Click);
            // 
            // WinCustomUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDn);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnPageDown);
            this.Controls.Add(this.btnPgDown);
            this.Controls.Add(this.gridControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "WinCustomUserControl";
            this.Size = new System.Drawing.Size(480, 640);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectJobType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSelectJobType;
        private DevExpress.XtraEditors.SimpleButton btnPgDown;
        private DevExpress.XtraEditors.SimpleButton btnPageDown;
        private DevExpress.XtraEditors.SimpleButton btnUp;
        private DevExpress.XtraEditors.SimpleButton btnDn;

    }
}
