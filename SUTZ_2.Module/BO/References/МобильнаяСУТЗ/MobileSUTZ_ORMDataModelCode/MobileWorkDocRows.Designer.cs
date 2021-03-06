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
namespace SUTZ_2.Module.BO.References.Mobile {

	[Persistent(@"Table_030")]
		[System.ComponentModel.DefaultProperty("BaseDoc")]
	public partial class MobileWorkDocRows : XPBaseObject {
		int fidd;
		[Key(true)]
		[Persistent(@"f001")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		public int idd {
			get { return fidd; }
			set { SetPropertyValue<int>("idd", ref fidd, value); }
		}
		SUTZ_2.Module.BO.References.Delimeters fDelimeter;
		[Persistent(@"f006")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Разделитель")]
		public SUTZ_2.Module.BO.References.Delimeters Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<SUTZ_2.Module.BO.References.Delimeters>("Delimeter", ref fDelimeter, value); }
		}
		Guid fidGUID;
		/// <summary>
		/// Идентификатор элемента, для синхронизации с СУТЗ
		/// </summary>
		[Persistent(@"f005")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"IDD SUTZ")]
		public Guid idGUID {
			get { return fidGUID; }
			set { SetPropertyValue<Guid>("idGUID", ref fidGUID, value); }
		}
		MobileWorkUnit fParentId;
		[Persistent(@"f002")]
		[DisplayName(@"единица работы")]
		[Association(@"MobileWorkDocRowsReferencesMobileWorkUnit")]
		public MobileWorkUnit ParentId {
			get { return fParentId; }
			set { SetPropertyValue<MobileWorkUnit>("ParentId", ref fParentId, value); }
		}
		int fBaseDocRowId;
		/// <summary>
		/// Идентификатор idd(int32) строки документа
		/// </summary>
		[Persistent(@"f004")]
		[DisplayName(@"IDD строки документа")]
		public int BaseDocRowId {
			get { return fBaseDocRowId; }
			set { SetPropertyValue<int>("BaseDocRowId", ref fBaseDocRowId, value); }
		}
		Guid fBaseDocRowGUID;
		/// <summary>
		/// Идентификатор строки базового документа
		/// </summary>
		[Persistent(@"f007")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"BaseDoc row GUID")]
		public Guid BaseDocRowGUID {
			get { return fBaseDocRowGUID; }
			set { SetPropertyValue<Guid>("BaseDocRowGUID", ref fBaseDocRowGUID, value); }
		}
		int fLineNo;
		[Persistent(@"f008")]
		[DisplayName(@"Line No")]
		public int LineNo {
			get { return fLineNo; }
			set { SetPropertyValue<int>("LineNo", ref fLineNo, value); }
		}
		bool fIsMarkDeleted;
		[Persistent(@"f009")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
	}

}
