using System;
using System.Linq;
using SUTZ_2.Module.Editors;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;

using DevExpress.Xpo;

namespace SUTZ_2.Win.Controls
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class Symbol_ScanBarcodeControl : DevExpress.XtraEditors.XtraUserControl, IXpoSessionAwareControl
    {
        private Session lokSession;

        public Symbol_ScanBarcodeControl()
        {
            InitializeComponent();
        }


        #region „лены IXpoSessionAwareControl

        void IXpoSessionAwareControl.UpdateDataSource(DevExpress.Xpo.Session session)
        {
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;
        }

        #endregion

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
