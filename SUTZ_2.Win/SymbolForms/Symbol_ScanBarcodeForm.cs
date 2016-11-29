using System;
using DevExpress.Xpo;
using SUTZ_2.Module.Editors;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;

namespace SUTZ_2.MobileSUTZ
{
    /// <summary>
    /// This is a custom WinForms form that displays persistent data received from XPO.
    /// You do not need to implement the IXpoSessionAwareControl interface if your form gets data from other sources or does not require data at all.
    /// </summary>
    public partial class Symbol_ScanBarcodeForm : DevExpress.XtraEditors.XtraForm {
        
        // свойство для определения режима выбора, в котором вызвана форма:
        private structScanStringParams structParams_;
        public structScanStringParams structParams
        {
            get { return structParams_; }
            set { structParams_ = value; }
        }        
        
        //  public Symbol_ScanBarcodeForm() {
        //    InitializeComponent();
        //}

        public Symbol_ScanBarcodeForm(structScanStringParams paramCaptionsDialog)
        {            
            structParams = paramCaptionsDialog;
            InitializeComponent();
            labelControl1.Text = paramCaptionsDialog.captionOne;
            labelControl2.Text = paramCaptionsDialog.captionTwo;           
        }

        private void Symbol_ScanBarcodeForm_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.txtScanBarcodeField.Focus();
        }

        private void Symbol_ScanBarcodeForm_Shown(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void txtScanBarcodeField_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //Debug.WriteLine("нажата кнопка: " + e.KeyChar+", введенный текст:"+txtScanBarcodeField.Text);
            if (e.KeyChar == '\r')
            {               
                structParams_.scanedBarcode = txtScanBarcodeField.Text;
                structParams_.successScan = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void Symbol_ScanBarcodeForm_Activated(object sender, EventArgs e)
        {
            this.txtScanBarcodeField.Focus();
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            structParams_.successScan = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
     }
}