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

	public partial class SUTZ_NET_DocJobeProperties : XPLiteObject {
		int frow_id;
		[Key(true)]
		public int row_id {
			get { return frow_id; }
			set { SetPropertyValue<int>("row_id", ref frow_id, value); }
		}
		Guid fiddSUTZ_GUID;
		/// <summary>
		/// Идентификатор документа в СУТЗ
		/// </summary>
		public Guid iddSUTZ_GUID {
			get { return fiddSUTZ_GUID; }
			set { SetPropertyValue<Guid>("iddSUTZ_GUID", ref fiddSUTZ_GUID, value); }
		}
		Guid fDelimeter;
		public Guid Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Guid>("Delimeter", ref fDelimeter, value); }
		}
		Guid fDocBaseID;
		[DisplayName(@"Идентификатор документа")]
		public Guid DocBaseID {
			get { return fDocBaseID; }
			set { SetPropertyValue<Guid>("DocBaseID", ref fDocBaseID, value); }
		}
		byte fMobileStatus;
		public byte MobileStatus {
			get { return fMobileStatus; }
			set { SetPropertyValue<byte>("MobileStatus", ref fMobileStatus, value); }
		}
		Guid fJobeType;
		public Guid JobeType {
			get { return fJobeType; }
			set { SetPropertyValue<Guid>("JobeType", ref fJobeType, value); }
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
		[DisplayName(@"Пометка удаления")]
		public bool isMarkForDeleted {
			get { return fisMarkForDeleted; }
			set { SetPropertyValue<bool>("isMarkForDeleted", ref fisMarkForDeleted, value); }
		}
	}

}
