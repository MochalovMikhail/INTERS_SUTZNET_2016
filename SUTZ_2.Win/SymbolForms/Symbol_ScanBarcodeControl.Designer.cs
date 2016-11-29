namespace SUTZ_2.Win.Controls
{
    partial class Symbol_ScanBarcodeControl
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
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.txtEditScanBarcode = new DevExpress.XtraEditors.TextEdit();
            this.labelTop1 = new DevExpress.XtraEditors.LabelControl();
            this.labelMiddle1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtEditScanBarcode.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(14, 210);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(186, 40);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "simpleButton1";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // txtEditScanBarcode
            // 
            this.txtEditScanBarcode.EditValue = "";
            this.txtEditScanBarcode.Location = new System.Drawing.Point(0, 171);
            this.txtEditScanBarcode.Name = "txtEditScanBarcode";
            this.txtEditScanBarcode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtEditScanBarcode.Properties.Appearance.Options.UseFont = true;
            this.txtEditScanBarcode.Size = new System.Drawing.Size(217, 26);
            this.txtEditScanBarcode.TabIndex = 1;
            // 
            // labelTop1
            // 
            this.labelTop1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelTop1.Location = new System.Drawing.Point(3, 5);
            this.labelTop1.Name = "labelTop1";
            this.labelTop1.Size = new System.Drawing.Size(217, 74);
            this.labelTop1.TabIndex = 2;
            this.labelTop1.Text = "UserQuestion";
            // 
            // labelMiddle1
            // 
            this.labelMiddle1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelMiddle1.Location = new System.Drawing.Point(2, 93);
            this.labelMiddle1.Name = "labelMiddle1";
            this.labelMiddle1.Size = new System.Drawing.Size(217, 74);
            this.labelMiddle1.TabIndex = 3;
            this.labelMiddle1.Text = "UserResponse";
            // 
            // Symbol_ScanBarcodeControl
            // 
            this.Controls.Add(this.labelMiddle1);
            this.Controls.Add(this.labelTop1);
            this.Controls.Add(this.txtEditScanBarcode);
            this.Controls.Add(this.simpleButton1);
            this.Name = "Symbol_ScanBarcodeControl";
            this.Size = new System.Drawing.Size(220, 260);
            ((System.ComponentModel.ISupportInitialize)(this.txtEditScanBarcode.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.TextEdit txtEditScanBarcode;
        private DevExpress.XtraEditors.LabelControl labelTop1;
        private DevExpress.XtraEditors.LabelControl labelMiddle1;
    }
}
