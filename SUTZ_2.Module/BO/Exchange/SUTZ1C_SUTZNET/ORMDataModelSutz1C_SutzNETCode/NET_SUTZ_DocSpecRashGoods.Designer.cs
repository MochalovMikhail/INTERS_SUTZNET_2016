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
namespace SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET {

	public partial class NET_SUTZ_DocSpecRashGoods : XPLiteObject {
		int frow_id;
		[Key(true)]
		public int row_id {
			get { return frow_id; }
			set { SetPropertyValue<int>("row_id", ref frow_id, value); }
		}
		NET_SUTZ_DocSpecRashHeaders fHeaderID;
		[Indexed(Name=@"Index_HeaderID")]
		[Association(@"NET_SUTZ_DocSpecRashGoodsReferencesNET_SUTZ_DocSpecRashHeaders")]
		public NET_SUTZ_DocSpecRashHeaders HeaderID {
			get { return fHeaderID; }
			set { SetPropertyValue<NET_SUTZ_DocSpecRashHeaders>("HeaderID", ref fHeaderID, value); }
		}
		int fDocLineNo;
		public int DocLineNo {
			get { return fDocLineNo; }
			set { SetPropertyValue<int>("DocLineNo", ref fDocLineNo, value); }
		}
		Guid? fGoodID;
		public Guid? GoodID {
			get { return fGoodID; }
			set { SetPropertyValue<Guid?>("GoodID", ref fGoodID, value); }
		}
		Guid? fUnitID;
		public Guid? UnitID {
			get { return fUnitID; }
			set { SetPropertyValue<Guid?>("UnitID", ref fUnitID, value); }
		}
		decimal? fQuantityOfUnits;
		public decimal? QuantityOfUnits {
			get { return fQuantityOfUnits; }
			set { SetPropertyValue<decimal?>("QuantityOfUnits", ref fQuantityOfUnits, value); }
		}
		decimal? fQuantityOfItems;
		public decimal? QuantityOfItems {
			get { return fQuantityOfItems; }
			set { SetPropertyValue<decimal?>("QuantityOfItems", ref fQuantityOfItems, value); }
		}
		decimal? fTotalQuantity;
		public decimal? TotalQuantity {
			get { return fTotalQuantity; }
			set { SetPropertyValue<decimal?>("TotalQuantity", ref fTotalQuantity, value); }
		}
		Guid? fTakeFrom;
		public Guid? TakeFrom {
			get { return fTakeFrom; }
			set { SetPropertyValue<Guid?>("TakeFrom", ref fTakeFrom, value); }
		}
		Guid? fPutTo;
		public Guid? PutTo {
			get { return fPutTo; }
			set { SetPropertyValue<Guid?>("PutTo", ref fPutTo, value); }
		}
		DateTime? fBestBefore;
		public DateTime? BestBefore {
			get { return fBestBefore; }
			set { SetPropertyValue<DateTime?>("BestBefore", ref fBestBefore, value); }
		}
		Guid? fPropertyID;
		public Guid? PropertyID {
			get { return fPropertyID; }
			set { SetPropertyValue<Guid?>("PropertyID", ref fPropertyID, value); }
		}
		Guid fDocLineUID;
		public Guid DocLineUID {
			get { return fDocLineUID; }
			set { SetPropertyValue<Guid>("DocLineUID", ref fDocLineUID, value); }
		}
		string fBarcode;
		[DisplayName(@"Штрих-код")]
		public string Barcode {
			get { return fBarcode; }
			set { SetPropertyValue<string>("Barcode", ref fBarcode, value); }
		}
		SUTZ_2.Module.BO.enDocRowMoveStatus fMoveStatus;
		[DisplayName(@"Статус частичного проведения")]
		public SUTZ_2.Module.BO.enDocRowMoveStatus MoveStatus {
			get { return fMoveStatus; }
			set { SetPropertyValue<SUTZ_2.Module.BO.enDocRowMoveStatus>("MoveStatus", ref fMoveStatus, value); }
		}
		Guid? fTransferPointID;
		[DisplayName(@"Точка передачи")]
		public Guid? TransferPointID {
			get { return fTransferPointID; }
			set { SetPropertyValue<Guid?>("TransferPointID", ref fTransferPointID, value); }
		}
		decimal fQuantityFact;
		/// <summary>
		/// Фактическое количество товара после размещения через моб.сутз
		/// </summary>
		[DisplayName(@"Кол.факт")]
		public decimal QuantityFact {
			get { return fQuantityFact; }
			set { SetPropertyValue<decimal>("QuantityFact", ref fQuantityFact, value); }
		}
	}

}
