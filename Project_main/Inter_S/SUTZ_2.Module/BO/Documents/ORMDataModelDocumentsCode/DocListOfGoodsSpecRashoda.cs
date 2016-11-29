using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocSpecRashodaGoods
    {
        public DocSpecRashodaGoods() : base(Session.DefaultSession) { }
        public DocSpecRashodaGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
