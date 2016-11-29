using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using System.Linq;

namespace SUTZ_2.Module.BO.References
{
    public partial class JobTypes : IBaseSUTZReferences
    {
        public JobTypes(Session session): base(session)
        {
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        // метод ищет элемент справочника виды работ по полю “ип–аботы, если не находит, то создает новый.
        //public JobTypes getJobTypeByEnumType(enTypeOfWorks enTypeWork)
        //{
        //    JobTypes elementSearch = null;
        //    Guard.ArgumentNotNull(enTypeWork, "enTypeWork");
        //    XPQuery<JobTypes> queryJobTypes = new XPQuery<JobTypes>(lokSession);
        //    elementSearch = (from c in queryJobTypes where c.TypeOfWork==enTypeWork select c).FirstOrDefault<JobTypes>() ;
        //    if (elementSearch==null)
        //    {
        //        elementSearch = new JobTypes(lokSession)
        //        {
        //            TypeOfWork = enTypeWork,
        //            idGUID = new Guid(),
        //            Delimeter = currentSessionSettings.defaultDelimeter,
        //            Description = enTypeWork.ToString(),
        //            IsEnabled = true
        //        };
        //        elementSearch.Save();
        //        lokSession.CommitTransaction();
        //    }
        //    return elementSearch;
        //}
    }
}
