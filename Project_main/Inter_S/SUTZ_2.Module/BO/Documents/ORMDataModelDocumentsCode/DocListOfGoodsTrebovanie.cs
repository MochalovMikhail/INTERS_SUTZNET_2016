using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocTrebovanieGoods
    {
        public DocTrebovanieGoods() : base(Session.DefaultSession) { }
        public DocTrebovanieGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
