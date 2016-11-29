using System;
using DevExpress.Xpo;
using SUTZ_2.Module.Editors;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO;

namespace SUTZ_2.Win
{
    /// <summary>
    /// This is a custom WinForms form that displays persistent data received from XPO.
    /// You do not need to implement the IXpoSessionAwareControl interface if your form gets data from other sources or does not require data at all.
    /// </summary>
    public partial class WinCustomForm : DevExpress.XtraEditors.XtraForm, IXpoSessionAwareControl {
        
        // свойство для определения режима выбора, в котором вызвана форма:
        private enVariantsOfSelectedForm workVariant;
        public enVariantsOfSelectedForm WorkVariant
        {
            get { return workVariant; }
            set { workVariant = value; }
        }

        private JobTypes jobType;
        public JobTypes JobType
        {
            get { return jobType; }
            set 
            { 
                jobType = value; 
                SymbolMainFormTemplate2 form1 = this.Owner as SymbolMainFormTemplate2;
                if (form1!=null)
                {
                    form1.onSuccesSelectJobType(jobType);
                }
            }
        }          

        public WinCustomForm() {
            InitializeComponent();
        }

        public WinCustomForm(enVariantsOfSelectedForm variantOfWork)
        {
            WorkVariant = variantOfWork;
            InitializeComponent();
        }

        #region IXpoSessionAwareControl Members
        public void UpdateDataSource(Session session) {
            //Initializing a child control when it is not created by XAF (placed on a custom form).
            ((IXpoSessionAwareControl)this.CustomUserControl).UpdateDataSource(session);
        }
        #endregion

        private void WinCustomForm_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void WinCustomForm_Shown(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.Text = "";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

        }
    }
}