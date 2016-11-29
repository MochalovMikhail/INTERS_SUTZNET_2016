using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using System.Linq;

namespace SUTZ_2.Module.BO.References
{

    public partial class PalletLabels : IBaseSUTZReferences
    {
        public PalletLabels(Session session) : base(session) 
        {
            Guard.ArgumentNotNull(session, "session");
            lokSession = session;
        }
        public override void AfterConstruction() 
        { 
            base.AfterConstruction(); 
        }
        public String Description
        {
            get { return Barcode; }
            set { Barcode = value; }
        }
        // метод выполняет разделение строки ШК из этикетки:
        // этикетка вида: 0001100000007173    
        public static String detectPalletBarcodeFromFullBarcode(String strBarcode)
        {
            strBarcode = strBarcode.Trim();
            if (strBarcode.StartsWith("00") && (strBarcode.Length >= 20))
            {
                return strBarcode.Substring(2, 16);
            }
            return strBarcode;
        }
        // Метод выполняет поиск этикетки по основному штрих-коду:
        //public PalletLabels findByBarcode(String strBarcode)
        //{
        //    // 1. поиск элемента справочника Штрихкоды товаров:
        //    PalletLabels palletLabelFinded = null;
        //    XPQuery<PalletLabels> queryPalletLabels = new XPQuery<PalletLabels>(lokSession);
        //    var listOfElements = from o in queryPalletLabels where o.Barcode == strBarcode select o;

        //    if (listOfElements.Count() == 0)
        //    {
        //        return palletLabelFinded;
        //    }
        //    else
        //    {
        //        palletLabelFinded = listOfElements.First();
        //    }
        //    return palletLabelFinded;
        //}
    }

}
