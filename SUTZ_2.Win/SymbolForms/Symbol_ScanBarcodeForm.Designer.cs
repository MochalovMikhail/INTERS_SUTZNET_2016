namespace SUTZ_2.MobileSUTZ
{
    partial class Symbol_ScanBarcodeForm
    {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.txtScanBarcodeField = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtScanBarcodeField.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Appearance.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.simpleButtonCancel.Appearance.Options.UseFont = true;
            this.simpleButtonCancel.Location = new System.Drawing.Point(12, 207);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(208, 44);
            this.simpleButtonCancel.TabIndex = 0;
            this.simpleButtonCancel.Text = "Отмена";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // txtScanBarcodeField
            // 
            this.txtScanBarcodeField.Location = new System.Drawing.Point(12, 175);
            this.txtScanBarcodeField.Name = "txtScanBarcodeField";
            this.txtScanBarcodeField.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtScanBarcodeField.Properties.Appearance.Options.UseFont = true;
            this.txtScanBarcodeField.Size = new System.Drawing.Size(207, 26);
            this.txtScanBarcodeField.TabIndex = 1;
            this.txtScanBarcodeField.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtScanBarcodeField_KeyPress);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl1.Location = new System.Drawing.Point(0, 0);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(232, 87);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Top text";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelControl2.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl2.Location = new System.Drawing.Point(0, 87);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(232, 82);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Top text";
            // 
            // Symbol_ScanBarcodeForm
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 263);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txtScanBarcodeField);
            this.Controls.Add(this.simpleButtonCancel);
            this.Name = "Symbol_ScanBarcodeForm";
            this.Text = "Scan barcode";
            this.Activated += new System.EventHandler(this.Symbol_ScanBarcodeForm_Activated);
            this.Load += new System.EventHandler(this.Symbol_ScanBarcodeForm_Load);
            this.Shown += new System.EventHandler(this.Symbol_ScanBarcodeForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtScanBarcodeField.Properties)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion        

        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.TextEdit txtScanBarcodeField;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;

    }
    
}