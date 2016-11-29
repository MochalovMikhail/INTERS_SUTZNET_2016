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
    public partial class XtraFormSymbolScanBarcodeHTMLLabel : DevExpress.XtraEditors.XtraForm        
    {
        private Session _session;
        private structScanStringParams structParams_;

        public structScanStringParams structParams 
        {
            get { return structParams_; }
            set {structParams_ = value;}
        }

        // делегаты для обработки ШК и получения заголовка окна:
        public Func<String, int> dlgProcessBarcode {get; set;}
        public Func<String> dlgGetLabelCaption { get; set; }

        public XtraFormSymbolScanBarcodeHTMLLabel(structScanStringParams paramCaptionsDialog)
        {
            structParams = paramCaptionsDialog;
            InitializeComponent();
            labelControlHTML.Text = paramCaptionsDialog.captionOne;
            txtScanBarcodeField.Text = "";
            //labelControlHTML.Text = paramCaptionsDialog.captionTwo;            

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
                txtScanBarcodeField.Properties.Mask.AutoComplete = AutoCompleteType.None;                  
                txtScanBarcodeField.Properties.Mask.BeepOnError = true;
                txtScanBarcodeField.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
                txtScanBarcodeField.Properties.Mask.EditMask = "d";
            }

            // 5. определение видимости кнопки вопроса:
            if (!paramCaptionsDialog.enableQuestionButton)
            {
                simpleButtonCancel.Visible = false;
            }
        }

        private void XtraFormSymbolScanBarcode_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.txtScanBarcodeField.Focus();

            if (dlgGetLabelCaption!=null)
            {
                labelControlHTML.Text = (String)dlgGetLabelCaption();
            }
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

        // основной метод для обработки сканирования ШК
        private void processBarcode()
        {
            // если указан делегат, вызов его по новой схеме
            if (dlgProcessBarcode != null)
            {
                int retValue = dlgProcessBarcode(txtScanBarcodeField.Text);
                if (retValue == 1)
                {
                    structParams_.scanedBarcode = txtScanBarcodeField.Text;
                    structParams_.successScan = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    labelControlHTML.Text = dlgGetLabelCaption();
                    txtScanBarcodeField.Text = "";
                }
            }
            else// делегат не указан, закрытие окна по старой схеме
            {
                structParams_.scanedBarcode = txtScanBarcodeField.Text;
                structParams_.successScan = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void txtScanBarcodeField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                processBarcode();
            }
        }
      
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //structScanStringParams newParams = new structScanStringParams()
            //{
            //    captionOne = "",
            //    captionTwo = structParams.captionHelp,
            //    disableCancelButton = true
            //};

            //XtraFormSymbolUserMessage newForm = new XtraFormSymbolUserMessage(newParams);
            //newForm.getLabelControl2().Font = new Font(newForm.getLabelControl2().Font.FontFamily, 24F);
            //newForm.ShowDialog();

            //return newForm.DialogResult;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButtonOK_Click(object sender, EventArgs e)
        {
            //structParams_.scanedBarcode = txtScanBarcodeField.Text;
            //structParams_.successScan = true;
            //this.DialogResult = DialogResult.OK;
            //this.Close();
            processBarcode();
        }

        private void txtScanBarcodeField_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}