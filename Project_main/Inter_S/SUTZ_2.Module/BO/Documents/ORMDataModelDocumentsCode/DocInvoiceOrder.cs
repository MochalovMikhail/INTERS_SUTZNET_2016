using System;
using DevExpress.Data.Filtering;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;

namespace SUTZ_2.Module.BO.Documents
{

    public partial class DocInvoiceOrder
    {
        //public DocInvoiceOrder() : base(Session.DefaultSession) { }
        public DocInvoiceOrder(Session session) : base(session) 
        {
            DocumentType = enDocumentType.ЗаявкаПокупателя;            
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        public override String getNameDoc()
        {
            return "Заявка покупателя";
        }

        protected override void OnSaved()
        {
            base.OnSaved();
            DocumentType = enDocumentType.ЗаявкаПокупателя;
        }

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
            if (Client != null)
            {
                Recipient = Client.ToString();
            }
            if (DocListOfGoods != null)
            {
                Quantity = DocListOfGoods.Sum(x => x.TotalQuantity);
            }
        }
    }

}
