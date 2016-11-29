using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocSpecPeremeshGoods
    {
        public DocSpecPeremeshGoods() : base(Session.DefaultSession) { }
        public DocSpecPeremeshGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
