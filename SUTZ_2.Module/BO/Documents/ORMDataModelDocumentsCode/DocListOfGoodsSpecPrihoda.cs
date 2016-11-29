using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocSpecPrihodaGoods
    {
        public DocSpecPrihodaGoods() : base(Session.DefaultSession) { }
        public DocSpecPrihodaGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
