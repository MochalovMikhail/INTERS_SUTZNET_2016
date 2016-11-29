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
    public partial class XtraFormSymbolUserMessage : DevExpress.XtraEditors.XtraForm
        
    {
        private Session _session {get ; set ;}        
        public structScanStringParams structParams {get ; set ;}

        // методы для получения контролов формы:
        public DevExpress.XtraEditors.LabelControl getLabelControl2()
        {
            return labelControl2;
        }

        public XtraFormSymbolUserMessage(structScanStringParams paramCaptionsDialog)
        {
            InitializeComponent();
            structParams = paramCaptionsDialog;            

            labelControlTop.Text = paramCaptionsDialog.captionOne;
            labelControl2.Text = paramCaptionsDialog.captionTwo;            

            if (_session == null)
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

            // 4. определение режима открытия формы:
            if (structParams.inputMode == enumInputMode.ТолькоСообщениеиОК)
            {
                simpleButtonCancel.Visible = false;
                simpleButtonOK.Text = "OK";
            }
            else if (structParams.inputMode == enumInputMode.ВопросДаНет)
            {
                simpleButtonOK.Text = "Да";
                simpleButtonCancel.Text = "Нет";
            }

            // 5. управление кнопками формы:
            if (structParams.disableCancelButton)
            {
                simpleButtonCancel.Visible = false;
            }
        }

        private void XtraFormSymbolScanBarcode_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void XtraFormSymbolScanBarcode_Shown(object sender, EventArgs e)
        {
            //this.ControlBox = false;
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void simpleButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}