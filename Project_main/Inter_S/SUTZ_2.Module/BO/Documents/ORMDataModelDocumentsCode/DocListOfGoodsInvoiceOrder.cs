using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocInvoiceOrderGoods
    {
        public DocInvoiceOrderGoods() : base(Session.DefaultSession) { }
        public DocInvoiceOrderGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
