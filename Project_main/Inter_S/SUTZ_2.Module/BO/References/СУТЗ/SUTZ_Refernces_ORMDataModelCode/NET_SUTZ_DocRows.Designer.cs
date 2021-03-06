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

	[Persistent(@"Table_033")]
	public partial class NET_SUTZ_DocRows : XPLiteObject {
		int frow_id;
		[Key(true)]
		[Persistent(@"f001")]
		public int row_id {
			get { return frow_id; }
			set { SetPropertyValue<int>("row_id", ref frow_id, value); }
		}
		NET_SUTZ_Unloads fParentId;
		[Indexed(Name=@"IndexParentId")]
		[Persistent(@"f002")]
		[Association(@"NET_SUTZ_DocRowsReferencesNET_SUTZ_Unloads")]
		public NET_SUTZ_Unloads ParentId {
			get { return fParentId; }
			set { SetPropertyValue<NET_SUTZ_Unloads>("ParentId", ref fParentId, value); }
		}
		int fObjectDocTableId;
		/// <summary>
		/// Идентификатор строки документа, если не указан, то выгружается весь документ
		/// </summary>
		[Persistent(@"f003")]
		[DisplayName(@"Ид строки документа")]
		public int ObjectDocTableId {
			get { return fObjectDocTableId; }
			set { SetPropertyValue<int>("ObjectDocTableId", ref fObjectDocTableId, value); }
		}
		bool fIsMarkDeleted;
		[Persistent(@"f004")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
		SUTZ_2.Module.BO.enFlagsOfOuboundFields fBitFlagsOfOuboundFields;
		[Persistent(@"f005")]
		[DisplayName(@"Битовая маска")]
		public SUTZ_2.Module.BO.enFlagsOfOuboundFields BitFlagsOfOuboundFields {
			get { return fBitFlagsOfOuboundFields; }
			set { SetPropertyValue<SUTZ_2.Module.BO.enFlagsOfOuboundFields>("BitFlagsOfOuboundFields", ref fBitFlagsOfOuboundFields, value); }
		}
	}

}
