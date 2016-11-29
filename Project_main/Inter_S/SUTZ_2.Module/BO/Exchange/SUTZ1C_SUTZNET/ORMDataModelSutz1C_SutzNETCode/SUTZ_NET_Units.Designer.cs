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

	public partial class SUTZ_NET_Units : XPLiteObject {
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
		int fiddSUTZ_int;
		public int iddSUTZ_int {
			get { return fiddSUTZ_int; }
			set { SetPropertyValue<int>("iddSUTZ_int", ref fiddSUTZ_int, value); }
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
		string fDescription;
		[Size(10)]
		public string Description {
			get { return fDescription; }
			set { SetPropertyValue<string>("Description", ref fDescription, value); }
		}
		Guid fOKEI;
		public Guid OKEI {
			get { return fOKEI; }
			set { SetPropertyValue<Guid>("OKEI", ref fOKEI, value); }
		}
		decimal fCoefficient;
		public decimal Coefficient {
			get { return fCoefficient; }
			set { SetPropertyValue<decimal>("Coefficient", ref fCoefficient, value); }
		}
		decimal fHeight;
		public decimal Height {
			get { return fHeight; }
			set { SetPropertyValue<decimal>("Height", ref fHeight, value); }
		}
		decimal fWidth;
		public decimal Width {
			get { return fWidth; }
			set { SetPropertyValue<decimal>("Width", ref fWidth, value); }
		}
		decimal fDepth;
		public decimal Depth {
			get { return fDepth; }
			set { SetPropertyValue<decimal>("Depth", ref fDepth, value); }
		}
		decimal fWeight;
		public decimal Weight {
			get { return fWeight; }
			set { SetPropertyValue<decimal>("Weight", ref fWeight, value); }
		}
		string fBarcode;
		[Size(50)]
		public string Barcode {
			get { return fBarcode; }
			set { SetPropertyValue<string>("Barcode", ref fBarcode, value); }
		}
		Guid fParentID;
		public Guid ParentID {
			get { return fParentID; }
			set { SetPropertyValue<Guid>("ParentID", ref fParentID, value); }
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
		bool fisMarkForDeleted;
		[DisplayName(@"������� ��������")]
		public bool isMarkForDeleted {
			get { return fisMarkForDeleted; }
			set { SetPropertyValue<bool>("isMarkForDeleted", ref fisMarkForDeleted, value); }
		}
	}

}
