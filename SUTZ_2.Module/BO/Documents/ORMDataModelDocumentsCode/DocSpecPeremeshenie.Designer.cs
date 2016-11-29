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

    [Persistent(@"Table_140")]
    public partial class DocSpecPeremeshenie : BaseDocument
    {
        SUTZ_2.Module.BO.References.StockRooms fStockRoomFrom;
        [Persistent(@"f050")]
        [DevExpress.Xpo.DisplayName(@"Кладовая-отправитель")]
        public SUTZ_2.Module.BO.References.StockRooms StockRoomFrom
        {
            get { return fStockRoomFrom; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.StockRooms>("StockRoomFrom", ref fStockRoomFrom, value); }
        }
        SUTZ_2.Module.BO.References.StockRooms fStockRoomTo;
        [Persistent(@"f051")]
        [DevExpress.Xpo.DisplayName(@"Кладовая-получатель")]
        public SUTZ_2.Module.BO.References.StockRooms StockRoomTo
        {
            get { return fStockRoomTo; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.StockRooms>("StockRoomTo", ref fStockRoomTo, value); }
        }
        bool fAllowMovement;
        [Persistent(@"f052")]
        [DevExpress.Xpo.DisplayName(@"Разрешить движение")]
        public bool AllowMovement
        {
            get { return fAllowMovement; }
            set { SetPropertyValue<bool>("AllowMovement", ref fAllowMovement, value); }
        }
        [Association(@"DocListOfGoodsSpecPeremeshReferencesDocSpecPeremeshenie"), Aggregated]
        public XPCollection<DocSpecPeremeshGoods> DocListOfGoods { get { return GetCollection<DocSpecPeremeshGoods>("DocListOfGoods"); } }
    }

}
