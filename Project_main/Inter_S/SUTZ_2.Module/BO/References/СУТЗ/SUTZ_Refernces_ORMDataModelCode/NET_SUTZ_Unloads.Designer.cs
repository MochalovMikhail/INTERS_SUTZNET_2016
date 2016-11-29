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

		/// <summary>
		/// Таблица объектов для выгрузки в СУТЗ 1С:7.7
		/// </summary>
	[Persistent(@"Table_032")]
	public partial class NET_SUTZ_Unloads : XPLiteObject {
		int frow_id;
		[Key(true)]
		[Persistent(@"f001")]
		public int row_id {
			get { return frow_id; }
			set { SetPropertyValue<int>("row_id", ref frow_id, value); }
		}
		string fObjectType;
		[Persistent(@"f002")]
		[DisplayName(@"Object Type")]
		public string ObjectType {
			get { return fObjectType; }
			set { SetPropertyValue<string>("ObjectType", ref fObjectType, value); }
		}
		int fObjectRow_Id;
		[Persistent(@"f003")]
		public int ObjectRow_Id {
			get { return fObjectRow_Id; }
			set { SetPropertyValue<int>("ObjectRow_Id", ref fObjectRow_Id, value); }
		}
		byte fOutload;
		[Indexed(Name=@"IndexOutload")]
		[Persistent(@"f005")]
		[DisplayName(@"Выгружен")]
		public byte Outload {
			get { return fOutload; }
			set { SetPropertyValue<byte>("Outload", ref fOutload, value); }
		}
		Users fUser;
		[Persistent(@"f006")]
		[DisplayName(@"Пользователь")]
		public Users User {
			get { return fUser; }
			set { SetPropertyValue<Users>("User", ref fUser, value); }
		}
		DateTime fdTimeInsertRecord;
		[Persistent(@"f007")]
		[DisplayName(@"Дата-время добавления строки задания")]
		public DateTime dTimeInsertRecord {
			get { return fdTimeInsertRecord; }
			set { SetPropertyValue<DateTime>("dTimeInsertRecord", ref fdTimeInsertRecord, value); }
		}
		DateTime? fdTimeUnloadRecord;
		[Persistent(@"f008")]
		[DisplayName(@"Дата-время выгрузки строки задания")]
		public DateTime? dTimeUnloadRecord {
			get { return fdTimeUnloadRecord; }
			set { SetPropertyValue<DateTime?>("dTimeUnloadRecord", ref fdTimeUnloadRecord, value); }
		}
		bool fFullDocTable;
		/// <summary>
		/// Флаг, определяет что выгружается в 7.7 - одна строка документа или полная табличная часть
		/// </summary>
		[Persistent(@"f009")]
		public bool FullDocTable {
			get { return fFullDocTable; }
			set { SetPropertyValue<bool>("FullDocTable", ref fFullDocTable, value); }
		}
		bool fIsMarkDeleted;
		[Persistent(@"f010")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
		[Association(@"NET_SUTZ_DocRowsReferencesNET_SUTZ_Unloads", typeof(NET_SUTZ_DocRows))]
		public XPCollection<NET_SUTZ_DocRows> NET_SUTZ_DocRows { get { return GetCollection<NET_SUTZ_DocRows>("NET_SUTZ_DocRows"); } }
	}

}
