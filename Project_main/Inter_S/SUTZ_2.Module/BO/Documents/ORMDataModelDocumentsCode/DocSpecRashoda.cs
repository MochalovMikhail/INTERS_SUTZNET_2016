using System;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocSpecRashoda
    {
        //public DocSpecRashoda() : base(Session.DefaultSession) { }
        public DocSpecRashoda(Session session) : base(session)
        {
            DocumentType = enDocumentType.СпецификацияРасхода;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public override String getNameDoc()
        {
            return "Спецификация расхода";
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            Sender = "";
            Recipient = "";
            Quantity = 0;
            if (StockRoomFrom != null)
            {
                Sender = StockRoomFrom.ToString();
            }
            if (DocBase != null)
            {
                if (DocBase.ClassInfo.ClassType==typeof(DocInvoiceOrder))
                {
                    Recipient = ((DocInvoiceOrder)DocBase).Client.ToString();
                }                
            }
            if (DocListOfGoods != null)
            {
                Quantity = DocListOfGoods.Sum(x => x.TotalQuantity);
            }
        }
    }
}
