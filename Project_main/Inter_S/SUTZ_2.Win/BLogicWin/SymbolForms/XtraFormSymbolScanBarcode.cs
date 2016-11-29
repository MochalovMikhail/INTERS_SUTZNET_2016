using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.ExpressApp.Xpo;
using SUTZ_2.Module;
using SUTZ_2.Module.BO.References;
using System.Diagnostics;
using DevExpress.Xpo;

namespace SUTZ_2.MobileSUTZ
{
    public partial class XtraFormSymbolScanBarcode : DevExpress.XtraEditors.XtraForm        
    {
        private Session _session;
        private structScanStringParams structParams_;

        public structScanStringParams structParams 
        {
            get { return structParams_; }
            set {structParams_ = value;}
        }

        public XtraFormSymbolScanBarcode(structScanStringParams paramCaptionsDialog)
        {
            structParams = paramCaptionsDialog;
            InitializeComponent();
            labelControl1.Text = paramCaptionsDialog.captionOne;
            labelControl2.Text = paramCaptionsDialog.captionTwo;            

            if (_session==null)
            {
                XPObjectSpace objectSpace = (XPObjectSpace)currentSessionSettings.ObjXafApp.CreateObjectSpace();
                _session = objectSpace.Session;
            }

            // 3. определение фонового цвета:            
            LogisticsSettings logSettings = new LogisticsSettings(_session);
            Color backColor = logSettings.getBackColorForMobileForms();
            if (!backColor.IsEmpty)
            {
                this.Appearance.BackColor = backColor;
            }     
           
            // 4. определение режима работы формы:
            if (paramCaptionsDialog.inputMode==enumInputMode.ВводТолькоДаты)
            {
                txtScanBarcodeField.Properties.Mask.AutoComplete = AutoCompleteType.None;                  txtScanBarcodeField.Properties.Mask.BeepOnError = true;
                txtScanBarcodeField.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
                txtScanBarcodeField.Properties.Mask.EditMask = "d";
            }

            // 5. определение видимости кнопки вопроса:
            if (!paramCaptionsDialog.enableQuestionButton)
            {
                buttonQuestion.Visible = false;
            }
        }

        private void XtraFormSymbolScanBarcode_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.txtScanBarcodeField.Focus();
        }

        private void XtraFormSymbolScanBarcode_Shown(object sender, EventArgs e)
        {
            //this.ControlBox = false;
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            structParams_.captionTwo = "";
            structParams_.successScan = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtScanBarcodeField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                structParams_.scanedBarcode = txtScanBarcodeField.Text;
                structParams_.successScan = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
      
        private void buttonQuestion_Click(object sender, EventArgs e)
        {
            structScanStringParams newParams = new structScanStringParams()
            {
                captionOne = "",
                captionTwo = structParams.captionHelp,
                disableCancelButton = true
            };

            XtraFormSymbolUserMessage newForm = new XtraFormSymbolUserMessage(newParams);
            newForm.getLabelControl2().Font = new Font(newForm.getLabelControl2().Font.FontFamily, 24F);
            newForm.ShowDialog();

            //return newForm.DialogResult;
        }
    }
}