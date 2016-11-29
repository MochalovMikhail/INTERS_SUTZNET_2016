using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
namespace SUTZ_2.Module.BO.References
{

    public partial class SetOfGoods : IBaseSUTZReferences
    {
        public SetOfGoods(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public String Description
        {
            get { return String.Format("{0}, {1} * {2}",Good,Unit,Quantity); }
            set { }
        }
    }

}
