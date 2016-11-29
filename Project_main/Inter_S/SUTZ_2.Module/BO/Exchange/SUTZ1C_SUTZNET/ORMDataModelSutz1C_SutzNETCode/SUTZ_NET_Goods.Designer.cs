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

	public partial class SUTZ_NET_Goods : XPLiteObject {
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
		string fCode;
		[Size(16)]
		public string Code {
			get { return fCode; }
			set { SetPropertyValue<string>("Code", ref fCode, value); }
		}
		string fArticle;
		[Size(16)]
		public string Article {
			get { return fArticle; }
			set { SetPropertyValue<string>("Article", ref fArticle, value); }
		}
		string fFullDescription;
		[Size(SizeAttribute.Unlimited)]
		public string FullDescription {
			get { return fFullDescription; }
			set { SetPropertyValue<string>("FullDescription", ref fFullDescription, value); }
		}
		Guid fDelimeter;
		public Guid Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Guid>("Delimeter", ref fDelimeter, value); }
		}
		string fDescription;
		public string Description {
			get { return fDescription; }
			set { SetPropertyValue<string>("Description", ref fDescription, value); }
		}
		bool fGroupExpDate;
		public bool GroupExpDate {
			get { return fGroupExpDate; }
			set { SetPropertyValue<bool>("GroupExpDate", ref fGroupExpDate, value); }
		}
		bool fEnableExpDate;
		public bool EnableExpDate {
			get { return fEnableExpDate; }
			set { SetPropertyValue<bool>("EnableExpDate", ref fEnableExpDate, value); }
		}
		int fFilterOfSelect;
		public int FilterOfSelect {
			get { return fFilterOfSelect; }
			set { SetPropertyValue<int>("FilterOfSelect", ref fFilterOfSelect, value); }
		}
		int fFilterOfMoving;
		public int FilterOfMoving {
			get { return fFilterOfMoving; }
			set { SetPropertyValue<int>("FilterOfMoving", ref fFilterOfMoving, value); }
		}
		bool fCalcByMoving;
		public bool CalcByMoving {
			get { return fCalcByMoving; }
			set { SetPropertyValue<bool>("CalcByMoving", ref fCalcByMoving, value); }
		}
		bool fIsASetOfGoods;
		public bool IsASetOfGoods {
			get { return fIsASetOfGoods; }
			set { SetPropertyValue<bool>("IsASetOfGoods", ref fIsASetOfGoods, value); }
		}
		Guid fBaseUnit;
		public Guid BaseUnit {
			get { return fBaseUnit; }
			set { SetPropertyValue<Guid>("BaseUnit", ref fBaseUnit, value); }
		}
		Guid fCountryOfProd;
		public Guid CountryOfProd {
			get { return fCountryOfProd; }
			set { SetPropertyValue<Guid>("CountryOfProd", ref fCountryOfProd, value); }
		}
		Guid fManufacturer;
		public Guid Manufacturer {
			get { return fManufacturer; }
			set { SetPropertyValue<Guid>("Manufacturer", ref fManufacturer, value); }
		}
		string fComment;
		[Size(SizeAttribute.Unlimited)]
		public string Comment {
			get { return fComment; }
			set { SetPropertyValue<string>("Comment", ref fComment, value); }
		}
		Guid fParentId;
		public Guid ParentId {
			get { return fParentId; }
			set { SetPropertyValue<Guid>("ParentId", ref fParentId, value); }
		}
		bool fIsFolder;
		public bool IsFolder {
			get { return fIsFolder; }
			set { SetPropertyValue<bool>("IsFolder", ref fIsFolder, value); }
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
		bool fCalcByVolume;
		public bool CalcByVolume {
			get { return fCalcByVolume; }
			set { SetPropertyValue<bool>("CalcByVolume", ref fCalcByVolume, value); }
		}
		bool fisMarkForDeleted;
		[DisplayName(@"������� ��������")]
		public bool isMarkForDeleted {
			get { return fisMarkForDeleted; }
			set { SetPropertyValue<bool>("isMarkForDeleted", ref fisMarkForDeleted, value); }
		}
	}

}
