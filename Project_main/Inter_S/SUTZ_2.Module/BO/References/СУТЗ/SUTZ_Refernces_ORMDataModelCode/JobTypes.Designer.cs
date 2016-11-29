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

	[Persistent(@"Table_012")]
		[System.ComponentModel.DefaultProperty("Description")]
	public partial class JobTypes : XPBaseObject {
		int fidd;
		[Key(true)]
		[Persistent(@"f001")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		public int idd {
			get { return fidd; }
			set { SetPropertyValue<int>("idd", ref fidd, value); }
		}
		int fCode;
		[Persistent(@"f002")]
		[DisplayName(@"Код")]
		public int Code {
			get { return fCode; }
			set { SetPropertyValue<int>("Code", ref fCode, value); }
		}
		string fDescription;
		[Persistent(@"f003")]
		[DisplayName(@"Наименование")]
		public string Description {
			get { return fDescription; }
			set { SetPropertyValue<string>("Description", ref fDescription, value); }
		}
		bool fIsEnabled;
		[Persistent(@"f006")]
		[DisplayName(@"Включен")]
		public bool IsEnabled {
			get { return fIsEnabled; }
			set { SetPropertyValue<bool>("IsEnabled", ref fIsEnabled, value); }
		}
		Delimeters fDelimeter;
		[Persistent(@"f004")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Разделитель")]
		public Delimeters Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Delimeters>("Delimeter", ref fDelimeter, value); }
		}
		SUTZ_2.Module.BO.enTypeOfWorks fTypeOfWork;
		[Persistent(@"f005")]
		[DevExpress.Persistent.Validation.RuleRequiredField(DevExpress.Persistent.Validation.DefaultContexts.Save)]
		[DisplayName(@"Тип работы")]
		public SUTZ_2.Module.BO.enTypeOfWorks TypeOfWork {
			get { return fTypeOfWork; }
			set { SetPropertyValue<SUTZ_2.Module.BO.enTypeOfWorks>("TypeOfWork", ref fTypeOfWork, value); }
		}
		Guid fidGUID;
		[Persistent(@"f007")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"id guid")]
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
		[Persistent(@"f008")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
	}

}
