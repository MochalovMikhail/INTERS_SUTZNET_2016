using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
namespace SUTZ_2.Module.BO.References
{

    public partial class Units : IBaseSUTZReferences
    {
        public Units(Session session) : base(session) {
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        protected override void OnSaved()
        {
            base.OnSaved();
            if (OKEI != null)
            {
                Description = String.Format("{0},{1}",OKEI,Koeff);
            }
        }
        public override string ToString()
        {
            //return Description;
            return String.Format("{0},{1}", OKEI, Koeff);
        }
    }
}
