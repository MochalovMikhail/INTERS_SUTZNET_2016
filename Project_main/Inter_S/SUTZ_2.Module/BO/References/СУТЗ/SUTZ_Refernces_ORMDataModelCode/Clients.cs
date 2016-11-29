using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.References
{

    public partial class Clients : IBaseSUTZReferences
    {
        public Clients(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public override string ToString()
        {
            return Description;
        }
    }
}
