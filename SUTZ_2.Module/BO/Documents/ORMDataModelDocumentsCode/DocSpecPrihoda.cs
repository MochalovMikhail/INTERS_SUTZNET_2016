using System;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocSpecPrihoda
    {
        //public DocSpecPrihoda() : base(Session.DefaultSession) { }
        public DocSpecPrihoda(Session session) : base(session) 
        {
            DocumentType = enDocumentType.СпецификацияПрихода;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public override String getNameDoc()
        {
            return "Спецификация прихода";
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            Sender = "";
            Recipient = "";
            Quantity = 0;
            //if (Warehouse != null)
            //{
            //    Sender = Warehouse.ToString();
            //}
            if (StockRoomTo != null)
            {
                Recipient = StockRoomTo.ToString();
            }
            if (DocListOfGoods != null)
            {
                Quantity = DocListOfGoods.Sum(x => x.TotalQuantity);
            }
        }
    }
}
