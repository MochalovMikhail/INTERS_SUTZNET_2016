//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
namespace SUTZ_2.Module.BO.Documents
{

    [Persistent(@"Table_130")]
    public partial class DocSpecPrihoda : BaseDocument
    {
        SUTZ_2.Module.BO.References.StockRooms fStockRoomTo;
        [Persistent(@"f050")]
        [DevExpress.Xpo.DisplayName(@"��������-����������")]
        public SUTZ_2.Module.BO.References.StockRooms StockRoomTo
        {
            get { return fStockRoomTo; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.StockRooms>("StockRoomTo", ref fStockRoomTo, value); }
        }
        bool fAllowSender;
        [Persistent(@"f051")]
        [DevExpress.Xpo.DisplayName(@"��������� �����������")]
        public bool AllowSender
        {
            get { return fAllowSender; }
            set { SetPropertyValue<bool>("AllowSender", ref fAllowSender, value); }
        }
        bool fAllowMovement;
        [Persistent(@"f052")]
        [DevExpress.Xpo.DisplayName(@"��������� ��������")]
        public bool AllowMovement
        {
            get { return fAllowMovement; }
            set { SetPropertyValue<bool>("AllowMovement", ref fAllowMovement, value); }
        }
        [Association(@"DocListOfGoodsSpecPrihodaReferencesDocSpecPrihoda"), Aggregated]
        public XPCollection<DocSpecPrihodaGoods> DocListOfGoods { get { return GetCollection<DocSpecPrihodaGoods>("DocListOfGoods"); } }
    }

}
