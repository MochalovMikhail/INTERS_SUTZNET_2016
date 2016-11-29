using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.References
{

    public partial class NET_SUTZ_DocRows
    {
        public NET_SUTZ_DocRows(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
