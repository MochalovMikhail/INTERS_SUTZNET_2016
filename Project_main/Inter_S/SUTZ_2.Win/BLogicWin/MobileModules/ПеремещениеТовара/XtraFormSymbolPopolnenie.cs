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
using NLog;

namespace SUTZ_2.MobileSUTZ
{
    public partial class XtraFormSymbolPopolnenie : DevExpress.XtraEditors.XtraForm
        
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Session _session;
        private structScanStringParams structParams_;
        public structScanStringParams structParams
        {
            get { return structParams_; }
            set { structParams_ = value; }
        }

        public XtraFormSymbolPopolnenie(structScanStringParams paramCaptionsDialog)
        {
            structParams = paramCaptionsDialog;
            InitializeComponent();
            // заголовки формы:
            labelControlMain.Text = "";
            labelControlTop.Text = paramCaptionsDialog.captionOne;

            // если передана ошибка, выводим ее в заголовоке на розовом фоне:
            if (paramCaptionsDialog.captionTwo.Length > 0)
            {
                labelControlMain.Text = paramCaptionsDialog.captionTwo;
                labelControlMain.Appearance.BackColor = Color.LightPink;
            }

            // количество план:
            textEditPlanUnits.Text = paramCaptionsDialog.decPlanUnits.ToString();
            textEditPlanItems.Text = paramCaptionsDialog.decPlanItems.ToString();
            textEditPlanTotal.Text = paramCaptionsDialog.decPlanTotal.ToString();

            // количество факт:
            textEditFactUnits.Text = paramCaptionsDialog.decFactUnits.ToString();
            textEditFactItems.Text = paramCaptionsDialog.decFactItems.ToString();
            textEditFactTotal.Text = paramCaptionsDialog.decFactTotal.ToString();            

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
            if (paramCaptionsDialog.inputMode==enumInputMode.ВводТолькоКоличестваФакт)
            {
                panelControlPlan.Visible = false;
            }
        }        
        
        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            structParams_.successScan = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void XtraFormSymbolInputQuantity_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.textEditFactUnits.Focus();
        }

        private void XtraFormSymbolInputQuantity_Shown(object sender, EventArgs e)
        {
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void simpleButtonОК_Click(object sender, EventArgs e)
        {
            structParams_.successScan = true;
            structParams_.decFactUnits = Decimal.Parse(textEditFactUnits.Text);
            structParams_.decFactItems = Decimal.Parse(textEditFactItems.Text);
            structParams_.decFactTotal = Decimal.Parse(textEditFactTotal.Text);

            structParams_.scanedBarcode = String.Format("КоличествоКор={0}, КоличествоШт={1}, Итого={2}", textEditFactUnits.Text, textEditFactItems.Text, textEditFactTotal.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void calculateTotalQuantity()
        {
            if (structParams.unitFactCalculate!=null)
            {
                Decimal decUnits = 0;
                Decimal decItems = 0;
                Decimal tekKoeff = (Decimal)structParams.unitFactCalculate.Koeff;

                try
                {
                    if (!Decimal.TryParse(textEditFactUnits.Text, out decUnits))
                    {
                        decUnits = 0;
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error("calculateTotalQuantity(): Ошибка: {0}, введенное значение textEditFactUnits.Text:{1}", ex.Message, textEditFactUnits.Text);
                }

                try
                {
                    if (!Decimal.TryParse(textEditFactItems.Text, out decItems))
                    {
                        decItems = 0;
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error("calculateTotalQuantity(): Ошибка: {0}, введенное значение textEditFactItems.Text:{1}", ex.Message, textEditFactItems.Text);
                }
                
                Decimal totalQuant = decUnits * tekKoeff + decItems;
                Decimal totalUnits = Math.Round(totalQuant / tekKoeff);
                Decimal totalItems = totalQuant - (totalUnits*tekKoeff);

                // Поле Общее количество:
                textEditFactTotal.Text = String.Format("{0}",totalQuant);
                //// Поле количество коробок:
                //textEditFactUnits.Text = String.Format("{0}", totalUnits);
                //// Поле количество штук:
                //textEditPlanItems.Text = String.Format("{0}", totalItems);
            }
        }
 
        private void textEditFactUnits_TextChanged(object sender, EventArgs e)
        {
            calculateTotalQuantity();
        }

        private void textEditFactItems_TextChanged(object sender, EventArgs e)
        {
            calculateTotalQuantity();
        }

        private void labelControlTop_Click(object sender, EventArgs e)
        {

        }

        private void simpleButtonHelp_Click(object sender, EventArgs e)
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