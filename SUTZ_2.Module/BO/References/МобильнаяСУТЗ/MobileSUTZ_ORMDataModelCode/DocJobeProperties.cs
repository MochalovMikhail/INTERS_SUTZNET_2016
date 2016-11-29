using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.References.Mobile
{

    public partial class DocJobesProperties
    {
        public DocJobesProperties(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
