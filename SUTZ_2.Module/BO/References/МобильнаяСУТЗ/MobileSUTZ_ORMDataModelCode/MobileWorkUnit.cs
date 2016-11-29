using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace SUTZ_2.Module.BO.References.Mobile
{

    public partial class MobileWorkUnit
    {
        public MobileWorkUnit(Session session) : base(session) 
        {
            idGUID = Guid.NewGuid();
        }

        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
