using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace SUTZ_2.Module.BO.References 
{
    public partial class BarcodesOfShipments : IBaseSUTZReferences
    {
        public BarcodesOfShipments(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public String Description
        {
            get {return Barcode;}
            set { }
        }
    }

}
