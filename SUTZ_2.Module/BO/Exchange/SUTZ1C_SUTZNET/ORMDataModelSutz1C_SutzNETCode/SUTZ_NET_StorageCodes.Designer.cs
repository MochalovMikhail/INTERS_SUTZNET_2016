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

	public partial class SUTZ_NET_StorageCodes : XPLiteObject {
		int frow_id;
		[Key(true)]
		public int row_id {
			get { return frow_id; }
			set { SetPropertyValue<int>("row_id", ref frow_id, value); }
		}
		Guid fiddSUTZ_guid;
		public Guid iddSUTZ_guid {
			get { return fiddSUTZ_guid; }
			set { SetPropertyValue<Guid>("iddSUTZ_guid", ref fiddSUTZ_guid, value); }
		}
		Guid fDelimeter;
		public Guid Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Guid>("Delimeter", ref fDelimeter, value); }
		}
		string fCode;
		[Size(16)]
		public string Code {
			get { return fCode; }
			set { SetPropertyValue<string>("Code", ref fCode, value); }
		}
		bool fIsFolder;
		public bool IsFolder {
			get { return fIsFolder; }
			set { SetPropertyValue<bool>("IsFolder", ref fIsFolder, value); }
		}
		Guid fParentID;
		public Guid ParentID {
			get { return fParentID; }
			set { SetPropertyValue<Guid>("ParentID", ref fParentID, value); }
		}
		int fRackNo;
		public int RackNo {
			get { return fRackNo; }
			set { SetPropertyValue<int>("RackNo", ref fRackNo, value); }
		}
		int fFloor;
		public int Floor {
			get { return fFloor; }
			set { SetPropertyValue<int>("Floor", ref fFloor, value); }
		}
		DateTime fdTimeWrite;
		public DateTime dTimeWrite {
			get { return fdTimeWrite; }
			set { SetPropertyValue<DateTime>("dTimeWrite", ref fdTimeWrite, value); }
		}
		SUTZ_NET_HostsUsers fUserHostWriter;
		public SUTZ_NET_HostsUsers UserHostWriter {
			get { return fUserHostWriter; }
			set { SetPropertyValue<SUTZ_NET_HostsUsers>("UserHostWriter", ref fUserHostWriter, value); }
		}
		byte fis_read;
		[Indexed(Name=@"Index_IsRead")]
		public byte is_read {
			get { return fis_read; }
			set { SetPropertyValue<byte>("is_read", ref fis_read, value); }
		}
		DateTime? fdTimeRead;
		public DateTime? dTimeRead {
			get { return fdTimeRead; }
			set { SetPropertyValue<DateTime?>("dTimeRead", ref fdTimeRead, value); }
		}
		SUTZ_NET_HostsUsers fUserHostReader;
		public SUTZ_NET_HostsUsers UserHostReader {
			get { return fUserHostReader; }
			set { SetPropertyValue<SUTZ_NET_HostsUsers>("UserHostReader", ref fUserHostReader, value); }
		}
		Guid fStockRoomID;
		public Guid StockRoomID {
			get { return fStockRoomID; }
			set { SetPropertyValue<Guid>("StockRoomID", ref fStockRoomID, value); }
		}
		int fCell;
		public int Cell {
			get { return fCell; }
			set { SetPropertyValue<int>("Cell", ref fCell, value); }
		}
		int fSubCell;
		public int SubCell {
			get { return fSubCell; }
			set { SetPropertyValue<int>("SubCell", ref fSubCell, value); }
		}
		string fPlace;
		[Size(2)]
		public string Place {
			get { return fPlace; }
			set { SetPropertyValue<string>("Place", ref fPlace, value); }
		}
		short fHeight;
		public short Height {
			get { return fHeight; }
			set { SetPropertyValue<short>("Height", ref fHeight, value); }
		}
		short fWidth;
		public short Width {
			get { return fWidth; }
			set { SetPropertyValue<short>("Width", ref fWidth, value); }
		}
		short fDepth;
		public short Depth {
			get { return fDepth; }
			set { SetPropertyValue<short>("Depth", ref fDepth, value); }
		}
		byte fCellState;
		public byte CellState {
			get { return fCellState; }
			set { SetPropertyValue<byte>("CellState", ref fCellState, value); }
		}
		Guid fGoodID;
		public Guid GoodID {
			get { return fGoodID; }
			set { SetPropertyValue<Guid>("GoodID", ref fGoodID, value); }
		}
		int fMaxItems;
		public int MaxItems {
			get { return fMaxItems; }
			set { SetPropertyValue<int>("MaxItems", ref fMaxItems, value); }
		}
		string fCodeOfCell;
		[Size(16)]
		public string CodeOfCell {
			get { return fCodeOfCell; }
			set { SetPropertyValue<string>("CodeOfCell", ref fCodeOfCell, value); }
		}
		bool fisMarkForDeleted;
		[DisplayName(@"������� ��������")]
		public bool isMarkForDeleted {
			get { return fisMarkForDeleted; }
			set { SetPropertyValue<bool>("isMarkForDeleted", ref fisMarkForDeleted, value); }
		}
	}

}
