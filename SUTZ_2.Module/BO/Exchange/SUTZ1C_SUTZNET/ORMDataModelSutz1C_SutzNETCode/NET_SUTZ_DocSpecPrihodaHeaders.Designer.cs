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

	public partial class NET_SUTZ_DocSpecPrihodaHeaders : XPLiteObject {
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
		string fDocNo;
		[Size(20)]
		public string DocNo {
			get { return fDocNo; }
			set { SetPropertyValue<string>("DocNo", ref fDocNo, value); }
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
		bool fFullDocTable;
		/// <summary>
		/// ����, ���������� ��� ����������� � 7.7 - ���� ������ ��������� ��� ������ ��������� �����
		/// </summary>
		public bool FullDocTable {
			get { return fFullDocTable; }
			set { SetPropertyValue<bool>("FullDocTable", ref fFullDocTable, value); }
		}
		bool fisMarkForDeleted;
		[DisplayName(@"������� ��������")]
		public bool isMarkForDeleted {
			get { return fisMarkForDeleted; }
			set { SetPropertyValue<bool>("isMarkForDeleted", ref fisMarkForDeleted, value); }
		}
		bool fAllowMovement;
		public bool AllowMovement {
			get { return fAllowMovement; }
			set { SetPropertyValue<bool>("AllowMovement", ref fAllowMovement, value); }
		}
		[Association(@"NET_SUTZ_DocSpecPrihodaGoodsReferencesNET_SUTZ_DocSpecPrihodaHeaders", typeof(NET_SUTZ_DocSpecPrihodaGoods))]
		public XPCollection<NET_SUTZ_DocSpecPrihodaGoods> NET_SUTZ_DocSpecPrihodaGoods { get { return GetCollection<NET_SUTZ_DocSpecPrihodaGoods>("NET_SUTZ_DocSpecPrihodaGoods"); } }
	}

}
