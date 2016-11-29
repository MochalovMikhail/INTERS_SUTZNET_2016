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

	[Persistent(@"Table_015")]
		[System.ComponentModel.DefaultProperty("Description")]
	public partial class TypesOfCellValue : XPBaseObject {
		int fidd;
		[Key(true)]
		[Persistent(@"f001")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		public int idd {
			get { return fidd; }
			set { SetPropertyValue<int>("idd", ref fidd, value); }
		}
		Delimeters fDelimeter;
		[Persistent(@"f002")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Разделитель")]
		public Delimeters Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Delimeters>("Delimeter", ref fDelimeter, value); }
		}
		int fHeight;
		[Persistent(@"f003")]
		[DisplayName(@"Высота")]
		public int Height {
			get { return fHeight; }
			set { SetPropertyValue<int>("Height", ref fHeight, value); }
		}
		int fWidth;
		[Persistent(@"f004")]
		[DisplayName(@"Ширина")]
		public int Width {
			get { return fWidth; }
			set { SetPropertyValue<int>("Width", ref fWidth, value); }
		}
		int fDepth;
		[Persistent(@"f005")]
		[DisplayName(@"Глубина")]
		public int Depth {
			get { return fDepth; }
			set { SetPropertyValue<int>("Depth", ref fDepth, value); }
		}
		string fDescription;
		[Size(50)]
		[Persistent(@"f006")]
		[DisplayName(@"Наименование")]
		public string Description {
			get { return fDescription; }
			set { SetPropertyValue<string>("Description", ref fDescription, value); }
		}
		Guid fidGUID;
		[Persistent(@"f007")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"id guid")]
		public Guid idGUID {
			get { return fidGUID; }
			set { SetPropertyValue<Guid>("idGUID", ref fidGUID, value); }
		}
		bool fIsMarkDeleted;
		[Persistent(@"f008")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
		[Association(@"StorageCodesReferencesTypesOfCellValue", typeof(StorageCodes))]
		public XPCollection<StorageCodes> StorageCodesCollection { get { return GetCollection<StorageCodes>("StorageCodesCollection"); } }
	}

}
