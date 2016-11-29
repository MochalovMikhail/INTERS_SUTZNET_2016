using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SUTZ_2.Module.CustomTemplates.Forms800x600
{
    public partial class SymbolFormSelectJobType : DevExpress.XtraEditors.XtraForm
    {
        public SymbolFormSelectJobType()
        {
            InitializeComponent();
        }

        private void SymbolFormSelectJobType_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void SymbolFormSelectJobType_Shown(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //System.Diagnostics.Debug.WriteLine("Вызов SymbolMainFormTemplate2_Shown");

        }
    }
}