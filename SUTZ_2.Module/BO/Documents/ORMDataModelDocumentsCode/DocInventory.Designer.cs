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

    [Persistent(@"Table_160")]
    public partial class DocInventory : BaseDocument
    {
        SUTZ_2.Module.BO.References.StockRooms fStockRoomTo;
        [Persistent(@"f050")]
        [DevExpress.Xpo.DisplayName(@"Кладовая")]
        public SUTZ_2.Module.BO.References.StockRooms StockRoomTo
        {
            get { return fStockRoomTo; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.StockRooms>("StockRoomTo", ref fStockRoomTo, value); }
        }
        SUTZ_2.Module.BO.enDocStatesInventory fTypeDoc;
        /// <summary>
        /// Вид инвентаризации - инвентаризация или корректировка
        /// </summary>
        public SUTZ_2.Module.BO.enDocStatesInventory TypeDoc
        {
            get { return fTypeDoc; }
            set { SetPropertyValue<SUTZ_2.Module.BO.enDocStatesInventory>("TypeDoc", ref fTypeDoc, value); }
        }
        SUTZ_2.Module.BO.References.Warehouses fWarehouse;
        /// <summary>
        /// Склад-отправитель
        /// </summary>
        [Persistent(@"f055")]
        [DevExpress.Xpo.DisplayName(@"Склад")]
        public SUTZ_2.Module.BO.References.Warehouses Warehouse
        {
            get { return fWarehouse; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Warehouses>("Warehouse", ref fWarehouse, value); }
        }
        [Association(@"DocListOfGoodsInventoryReferencesDocInventory"), Aggregated]
        public XPCollection<DocInventoryGoods> DocListOfGoods { get { return GetCollection<DocInventoryGoods>("DocListOfGoods"); } }
    }

}
