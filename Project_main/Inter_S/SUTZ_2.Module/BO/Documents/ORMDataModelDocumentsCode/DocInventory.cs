using System;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocInventory
    {
        //public DocInventory() : base(Session.DefaultSession) { }
        public DocInventory(Session session) : base(session) 
        {
            DocumentType = enDocumentType.ѕересчет—клада;

        }
        public override void AfterConstruction() { base.AfterConstruction(); }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            Sender = "";
            Recipient = "";
            Quantity = 0;
            if (Warehouse != null)
            {
                Sender = Warehouse.ToString();
            }
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
