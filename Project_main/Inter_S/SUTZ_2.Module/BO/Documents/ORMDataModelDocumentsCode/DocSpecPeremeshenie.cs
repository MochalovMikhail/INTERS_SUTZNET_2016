using System;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocSpecPeremeshenie
    {
        //public DocSpecPeremeshenie() : base(Session.DefaultSession) { }
        public DocSpecPeremeshenie(Session session) : base(session) 
        {
            DocumentType = enDocumentType.СпецификацияПеремещения;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public override String getNameDoc()
        {
            return "Спецификация перемещения";
        }

    }

}
