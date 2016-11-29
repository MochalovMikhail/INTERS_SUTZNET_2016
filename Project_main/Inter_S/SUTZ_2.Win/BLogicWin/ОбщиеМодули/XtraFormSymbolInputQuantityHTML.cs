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
using SUTZ_2.Module.BO.References.Mobile;

namespace SUTZ_2.MobileSUTZ
{
    public partial class XtraFormSymbolInputQuantityHTML : DevExpress.XtraEditors.XtraForm        
    {
        private Session _session;
        public structScanStringParams structParams;

        //public structScanStringParams structParams 
        //{
        //    get { return structParams_; }
        //    set {structParams_ = value;}
        //}

        // делегаты для обработки ШК и получения заголовка окна:
        //public delegateProcessBarcode dlgProcessBarcode {get; set;}
        public Func<string, int> dlgProcessBarcode { get; set; }
        public Func<String> dlgGetLabelCaption { get; set; }

        public XtraFormSymbolInputQuantityHTML(structScanStringParams paramCaptionsDialog)
        {
            structParams = paramCaptionsDialog;
            InitializeComponent();
            labelControlMain.Text = paramCaptionsDialog.captionOne;
            //txtScanBarcodeField.Text = "";
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
                //txtScanBarcodeField.Properties.Mask.AutoComplete = AutoCompleteType.None;                  
                //txtScanBarcodeField.Properties.Mask.BeepOnError = true;
                //txtScanBarcodeField.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
                //txtScanBarcodeField.Properties.Mask.EditMask = "d";
            }

            // 5. определение видимости кнопки вопроса:
            if (!paramCaptionsDialog.enableQuestionButton)
            {
                simpleButtonError.Visible = false;
            }

            //labelControlPlanUnits.DataBindings.Add("Text", structParams, "captionOne");
            labelControlPlanUnits.Text = structParams.decPlanUnits.ToString("0.##");
            labelControlPlanItems.Text = structParams.decPlanItems.ToString("0.##");
            labelControlPlanTotal.Text = structParams.decPlanTotal.ToString("0.##");

            textEditFactUnits.Text = structParams.decFactUnits.ToString("0.##");
            textEditFactItems.Text = structParams.decFactItems.ToString("0.##");
            textEditFactTotal.Text = structParams.decFactTotal.ToString("0.##");

            if (structParams.unitFactCalculate!=null)
            {
                labelControlCaptionUnits.Text = structParams.unitFactCalculate.ToString();
            }            

            labelControlTop.Text = structParams.captionOne;
            labelControlMain.Text = structParams.captionTwo;
            labelControlMessage.Text = "";
        }

        private void XtraFormSymbolScanBarcode_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //this.txtScanBarcodeField.Focus();

            //if (dlgGetLabelCaption!=null)
            //{
            //    labelControlMain.Text = (String)dlgGetLabelCaption();
            //}
        }

        private void XtraFormSymbolScanBarcode_Shown(object sender, EventArgs e)
        {
            //this.ControlBox = false;
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            structParams.captionTwo = "";
            structParams.successScan = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // основной метод для обработки сканирования ШК
        private void processBarcode()
        {
            // если указан делегат, вызов его по новой схеме
            if (dlgProcessBarcode != null)
            {
                //int retValue = dlgProcessBarcode(txtScanBarcodeField.Text);
                //if (retValue == 1)
                //{
                //    structParams_.scanedBarcode = txtScanBarcodeField.Text;
                //    structParams_.successScan = true;
                //    this.DialogResult = DialogResult.OK;
                //    this.Close();
                //}
                //else
                //{
                //    labelControlMain.Text = dlgGetLabelCaption();
                //    txtScanBarcodeField.Text = "";
                //}
            }
            else// делегат не указан, закрытие окна по старой схеме
            {
                //structParams_.scanedBarcode = txtScanBarcodeField.Text;
                //structParams_.successScan = true;
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

        }

        private void simpleButtonOK_Click(object sender, EventArgs e)
        {
            //structParams_.scanedBarcode = txtScanBarcodeField.Text;
            //structParams_.successScan = true;
            //this.DialogResult = DialogResult.OK;
            //this.Close();
            //processBarcode();

            // 1. сравним фактическое количество с плановым:
            decimal totalFact = calculateTotalFactQuantity();
            
            // 2. количество факт больше, чем плановое
            if (totalFact>structParams.decPlanTotal)
            {
                labelControlMessage.BackColor = Color.Red;
                //labelControlMessage.ForeColor = Color.Red;
                labelControlMessage.Text = "Превышение количества!";
            }
            else if ((totalFact < structParams.decPlanTotal)&&(structParams.iddMobileErrors==0))
            {                
                labelControlMessage.BackColor = Color.Red;
                labelControlMessage.Text = "Товара не хватает!" + Environment.NewLine + " Выберите ошибку.";
            }
            else
            {
                structParams.successScan = true;
                structParams.decFactTotal = calculateTotalFactQuantity();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void textEditFactItems_EditValueChanged(object sender, EventArgs e)
        {
            calculateTotalFactQuantity();
        }

        private void textEditFactUnits_EditValueChanged(object sender, EventArgs e)
        {
            calculateTotalFactQuantity();
        }

        private decimal calculateTotalFactQuantity()
        {
            Decimal unitsFact = 0;
            Decimal itemsFact = 0;

            if (!Decimal.TryParse(textEditFactItems.Text, out itemsFact))
                return 0;

            if (structParams.unitFactCalculate!=null)
            {
                if (!Decimal.TryParse(textEditFactUnits.Text,out unitsFact))
                    return 0;
                if (structParams.unitFactCalculate==null)
                    return 0;
                Decimal totalItems = unitsFact*(decimal)structParams.unitFactCalculate.Koeff + itemsFact;

                textEditFactTotal.Text = totalItems.ToString("0.##");

                return totalItems;
            }
            return itemsFact;
        }

        private void simpleButtonError_Click(object sender, EventArgs e)
        {
            // 1. вызов диалога выбора ошибки
            UserSelect userSelect = new UserSelect();
            DialogResult dlgResult = userSelect.inputValueFromType(typeof(MobileErrors));
            if (dlgResult == DialogResult.OK)
            {
                // 1.2 установка ошибки в переданной структуре
                if (userSelect.selectValue>0)
                {
                    MobileErrors mError = (MobileErrors)_session.GetObjectByKey(typeof(MobileErrors), userSelect.selectValue, false);
                    if (mError!=null)
                    {
                        structParams.iddMobileErrors = userSelect.selectValue;

                        labelControlMessage.BackColor = Color.Red;
                        labelControlMessage.Text = mError.Description;
                    }                    
                }                
            }
        }
    }
}