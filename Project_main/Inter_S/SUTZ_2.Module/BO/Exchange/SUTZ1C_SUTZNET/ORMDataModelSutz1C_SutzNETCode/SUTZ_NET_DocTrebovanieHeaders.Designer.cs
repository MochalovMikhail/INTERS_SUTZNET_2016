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

	public partial class SUTZ_NET_DocTrebovanieHeaders : XPLiteObject {
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
		string fDocNo;
		[Size(20)]
		public string DocNo {
			get { return fDocNo; }
			set { SetPropertyValue<string>("DocNo", ref fDocNo, value); }
		}
		byte fStatusDoc;
		public byte StatusDoc {
			get { return fStatusDoc; }
			set { SetPropertyValue<byte>("StatusDoc", ref fStatusDoc, value); }
		}
		DateTime fDocDateTime;
		public DateTime DocDateTime {
			get { return fDocDateTime; }
			set { SetPropertyValue<DateTime>("DocDateTime", ref fDocDateTime, value); }
		}
		string fComment;
		[Size(SizeAttribute.Unlimited)]
		public string Comment {
			get { return fComment; }
			set { SetPropertyValue<string>("Comment", ref fComment, value); }
		}
		Guid fDocBaseID;
		public Guid DocBaseID {
			get { return fDocBaseID; }
			set { SetPropertyValue<Guid>("DocBaseID", ref fDocBaseID, value); }
		}
		string fDocBaseInString;
		[Size(50)]
		public string DocBaseInString {
			get { return fDocBaseInString; }
			set { SetPropertyValue<string>("DocBaseInString", ref fDocBaseInString, value); }
		}
		Guid fClientID;
		public Guid ClientID {
			get { return fClientID; }
			set { SetPropertyValue<Guid>("ClientID", ref fClientID, value); }
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
		[Association(@"SUTZ_NET_DocTrebovanieGoodsReferencesSUTZ_NET_DocTrebovanieHeaders", typeof(SUTZ_NET_DocTrebovanieGoods))]
		public XPCollection<SUTZ_NET_DocTrebovanieGoods> SUTZ_NET_DocTrebovanieGoodsCollection { get { return GetCollection<SUTZ_NET_DocTrebovanieGoods>("SUTZ_NET_DocTrebovanieGoodsCollection"); } }
	}

}
