using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocInventoryGoods
    {
        public DocInventoryGoods() : base(Session.DefaultSession) { }
        public DocInventoryGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
