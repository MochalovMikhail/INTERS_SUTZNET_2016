using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using System.Linq;

namespace SUTZ_2.Module.BO.References
{
    public partial class Goods : IBaseSUTZReferences
    {
        public Goods(Session session):base(session)
        {
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        public override string ToString()
        {
            return FullDescription;// hh:mm:ss.fff tt
        }

        public String Description
        {
            get { return ShortDescription; }
            set
            {
                if (value != null)
                {
                    ShortDescription = value;
                }
            }
        }

   }
}

 
