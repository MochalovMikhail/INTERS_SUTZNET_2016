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
namespace SUTZ_2.Module.BO.References {

	[OptimisticLocking(OptimisticLockingBehavior.LockModified)]
	[Persistent(@"Table_010")]
		[System.ComponentModel.DefaultProperty("Code")]
	public partial class StorageCodes : XPBaseObject {
		int fidd;
		[Key(true)]
		[Persistent(@"f001")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		public int idd {
			get { return fidd; }
			set { SetPropertyValue<int>("idd", ref fidd, value); }
		}
		Delimeters fDelimeter;
		[Persistent(@"f002")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Разделитель")]
		public Delimeters Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Delimeters>("Delimeter", ref fDelimeter, value); }
		}
		string fCode;
		[Indexed(Name=@"IndexCode")]
		[Size(30)]
		[Persistent(@"f003")]
		[DisplayName(@"Код")]
		public string Code {
			get { return fCode; }
			set { SetPropertyValue<string>("Code", ref fCode, value); }
		}
		string fPalletCode;
		[Size(30)]
		[Persistent(@"f004")]
		public string PalletCode {
			get { return fPalletCode; }
			set { SetPropertyValue<string>("PalletCode", ref fPalletCode, value); }
		}
		bool fIsFolder;
		[Persistent(@"f005")]
		[DisplayName(@"Это группа")]
		public bool IsFolder {
			get { return fIsFolder; }
			set { SetPropertyValue<bool>("IsFolder", ref fIsFolder, value); }
		}
		StorageCodes fParentID;
		[Persistent(@"f006")]
		[DisplayName(@"Родитель")]
		[Association(@"Table_010ReferencesTable_010")]
		public StorageCodes ParentID {
			get { return fParentID; }
			set { SetPropertyValue<StorageCodes>("ParentID", ref fParentID, value); }
		}
		StockRooms fStockroom;
		[Persistent(@"f007")]
		[DisplayName(@"Кладовая")]
		public StockRooms Stockroom {
			get { return fStockroom; }
			set { SetPropertyValue<StockRooms>("Stockroom", ref fStockroom, value); }
		}
		int fStillage;
		[Persistent(@"f008")]
		[DisplayName(@"Стеллаж")]
		public int Stillage {
			get { return fStillage; }
			set { SetPropertyValue<int>("Stillage", ref fStillage, value); }
		}
		int fFloor;
		[Persistent(@"f009")]
		[DisplayName(@"Этаж")]
		public int Floor {
			get { return fFloor; }
			set { SetPropertyValue<int>("Floor", ref fFloor, value); }
		}
		int fCell;
		[Persistent(@"f010")]
		[DisplayName(@"Ячейка")]
		public int Cell {
			get { return fCell; }
			set { SetPropertyValue<int>("Cell", ref fCell, value); }
		}
		int fSubCell;
		[Persistent(@"f011")]
		[DisplayName(@"Подячейка")]
		public int SubCell {
			get { return fSubCell; }
			set { SetPropertyValue<int>("SubCell", ref fSubCell, value); }
		}
		TypesOfCellValue fCellType;
		[Persistent(@"f012")]
		[DisplayName(@"Тип ячейки")]
		[Association(@"StorageCodesReferencesTypesOfCellValue")]
		public TypesOfCellValue CellType {
			get { return fCellType; }
			set { SetPropertyValue<TypesOfCellValue>("CellType", ref fCellType, value); }
		}
		SUTZ_2.Module.BO.enCellStates fCellState;
		[Persistent(@"f013")]
		[DisplayName(@"Состояние ячейки")]
		public SUTZ_2.Module.BO.enCellStates CellState {
			get { return fCellState; }
			set { SetPropertyValue<SUTZ_2.Module.BO.enCellStates>("CellState", ref fCellState, value); }
		}
		Goods fGood;
		[Persistent(@"f014")]
		[DisplayName(@"Товар")]
		public Goods Good {
			get { return fGood; }
			set { SetPropertyValue<Goods>("Good", ref fGood, value); }
		}
		int fMaxItems;
		[Persistent(@"f015")]
		[DisplayName(@"Макс.штук")]
		public int MaxItems {
			get { return fMaxItems; }
			set { SetPropertyValue<int>("MaxItems", ref fMaxItems, value); }
		}
		Guid fidGUID;
		[Persistent(@"f017")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		public Guid idGUID {
			get { return fidGUID; }
			set { SetPropertyValue<Guid>("idGUID", ref fidGUID, value); }
		}
		DevExpress.Xpo.Session flokSession;
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[MemberDesignTimeVisibility(false)]
		[NonPersistent]
		public DevExpress.Xpo.Session lokSession {
			get { return flokSession; }
			set { SetPropertyValue<DevExpress.Xpo.Session>("lokSession", ref flokSession, value); }
		}
		bool fIsMarkDeleted;
		[Persistent(@"f018")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
		[Association(@"Table_010ReferencesTable_010", typeof(StorageCodes))]
		public XPCollection<StorageCodes> Table_010Collection { get { return GetCollection<StorageCodes>("Table_010Collection"); } }
	}

}
