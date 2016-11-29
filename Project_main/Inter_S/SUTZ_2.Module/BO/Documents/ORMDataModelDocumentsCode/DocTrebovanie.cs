using System;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocTrebovanie
    {
        //public DocTrebovanie() : base(Session.DefaultSession) { }
        public DocTrebovanie(Session session) : base(session) 
        {
            DocumentType = enDocumentType.ТребованиеНаРазгрузку;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }
        public override String getNameDoc()
        {
            return "Требование на разгрузку";
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
            //if (StockRoomTo != null)
            //{
            //    Recipient = StockRoomTo.ToString();
            //}
            if (DocTrebovanieGoodsCollection != null)
            {
                Quantity = DocTrebovanieGoodsCollection.Sum(x => x.TotalQuantity);
            }
        }

    }

}
