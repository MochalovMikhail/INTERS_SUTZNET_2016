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

	[Persistent(@"Table_020")]
		[System.ComponentModel.DefaultProperty("ShortDescription")]
	public partial class Goods : XPBaseObject {
		int fidd;
		[Key(true)]
		[Persistent(@"f001")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		public int idd {
			get { return fidd; }
			set { SetPropertyValue<int>("idd", ref fidd, value); }
		}
		int fCode;
		[Indexed(Name=@"IndexCode")]
		[Persistent(@"f002")]
		[DisplayName(@"Код")]
		public int Code {
			get { return fCode; }
			set { SetPropertyValue<int>("Code", ref fCode, value); }
		}
		string fArticle;
		[Size(16)]
		[Persistent(@"f003")]
		[DisplayName(@"Артикул")]
		public string Article {
			get { return fArticle; }
			set { SetPropertyValue<string>("Article", ref fArticle, value); }
		}
		string fFullDescription;
		[Size(SizeAttribute.Unlimited)]
		[Persistent(@"f004")]
		[DisplayName(@"Полное наименование")]
		public string FullDescription {
			get { return fFullDescription; }
			set { SetPropertyValue<string>("FullDescription", ref fFullDescription, value); }
		}
		Delimeters fDelimeter;
		[Persistent(@"f005")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Разделитель")]
		public Delimeters Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<Delimeters>("Delimeter", ref fDelimeter, value); }
		}
		string fShortDescription;
		[Persistent(@"f006")]
		[DisplayName(@"Наименование")]
		public string ShortDescription {
			get { return fShortDescription; }
			set { SetPropertyValue<string>("ShortDescription", ref fShortDescription, value); }
		}
		bool fGroupExpDate;
		[Persistent(@"f007")]
		[DisplayName(@"Группировать сроки годности в отборе")]
		public bool GroupExpDate {
			get { return fGroupExpDate; }
			set { SetPropertyValue<bool>("GroupExpDate", ref fGroupExpDate, value); }
		}
		bool fEnableExpDate;
		[Persistent(@"f008")]
		[DisplayName(@"Вкл.учет по срокам годности")]
		public bool EnableExpDate {
			get { return fEnableExpDate; }
			set { SetPropertyValue<bool>("EnableExpDate", ref fEnableExpDate, value); }
		}
		int fFilterOfSelect;
		[Persistent(@"f009")]
		[DisplayName(@"Фильтр отбора")]
		public int FilterOfSelect {
			get { return fFilterOfSelect; }
			set { SetPropertyValue<int>("FilterOfSelect", ref fFilterOfSelect, value); }
		}
		int fFilterOfMoving;
		[Persistent(@"f010")]
		[DisplayName(@"Фильтр пополнения")]
		public int FilterOfMoving {
			get { return fFilterOfMoving; }
			set { SetPropertyValue<int>("FilterOfMoving", ref fFilterOfMoving, value); }
		}
		bool fCalcByVolume;
		[Persistent(@"f011")]
		[DisplayName(@"Расчет по объему")]
		public bool CalcByVolume {
			get { return fCalcByVolume; }
			set { SetPropertyValue<bool>("CalcByVolume", ref fCalcByVolume, value); }
		}
		bool fIsASetOfGoods;
		[Persistent(@"f012")]
		[DisplayName(@"Это набор")]
		public bool IsASetOfGoods {
			get { return fIsASetOfGoods; }
			set { SetPropertyValue<bool>("IsASetOfGoods", ref fIsASetOfGoods, value); }
		}
		int fIddSUTZ_int;
		[Persistent(@"f013")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"iddSUTZ int")]
		public int IddSUTZ_int {
			get { return fIddSUTZ_int; }
			set { SetPropertyValue<int>("IddSUTZ_int", ref fIddSUTZ_int, value); }
		}
		Units fBaseUnit;
		[Persistent(@"f014")]
		[DisplayName(@"Базовая единица измерения")]
		public Units BaseUnit {
			get { return fBaseUnit; }
			set { SetPropertyValue<Units>("BaseUnit", ref fBaseUnit, value); }
		}
		Countries fCountryOfProd;
		[Persistent(@"f015")]
		[DisplayName(@"Страна производитель")]
		public Countries CountryOfProd {
			get { return fCountryOfProd; }
			set { SetPropertyValue<Countries>("CountryOfProd", ref fCountryOfProd, value); }
		}
		Manufacturers fManufacturer;
		[Persistent(@"f016")]
		[DisplayName(@"Фирма-производитель")]
		public Manufacturers Manufacturer {
			get { return fManufacturer; }
			set { SetPropertyValue<Manufacturers>("Manufacturer", ref fManufacturer, value); }
		}
		string fComment;
		[Size(SizeAttribute.Unlimited)]
		[Persistent(@"f017")]
		[DisplayName(@"Комментарий")]
		public string Comment {
			get { return fComment; }
			set { SetPropertyValue<string>("Comment", ref fComment, value); }
		}
		Goods fParentID;
		[Persistent(@"f018")]
		[DisplayName(@"Родитель")]
		[Association(@"GoodsReferencesGoods")]
		public Goods ParentID {
			get { return fParentID; }
			set { SetPropertyValue<Goods>("ParentID", ref fParentID, value); }
		}
		bool fIsFolder;
		[Persistent(@"f019")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Это группа")]
		public bool IsFolder {
			get { return fIsFolder; }
			set { SetPropertyValue<bool>("IsFolder", ref fIsFolder, value); }
		}
		Guid fidGUID;
		[Persistent(@"f020")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"idd guid")]
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
		[Persistent(@"f021")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
		[Association(@"GoodsReferencesGoods", typeof(Goods))]
		public XPCollection<Goods> GoodsCollection { get { return GetCollection<Goods>("GoodsCollection"); } }
		[DisplayName(@"Единицы измерения")]
		[Association(@"UnitsReferencesGoods", typeof(Units)), Aggregated]
		public XPCollection<Units> UnitsCollection { get { return GetCollection<Units>("UnitsCollection"); } }
		[DisplayName(@"Компектация")]
		[Association(@"SetOfGoodsReferencesGoods", typeof(SetOfGoods)), Aggregated]
		public XPCollection<SetOfGoods> SetOfGoodsCollection { get { return GetCollection<SetOfGoods>("SetOfGoodsCollection"); } }
		[DisplayName(@"Штрих-коды товара")]
		[Association(@"BarcodesReferencesGoods", typeof(BarcodesOfGoods)), Aggregated]
		public XPCollection<BarcodesOfGoods> BarcodesCollection { get { return GetCollection<BarcodesOfGoods>("BarcodesCollection"); } }
		[DisplayName(@"Характеристики номенклатуры")]
		[Association(@"PropertiesOfGoodsReferencesGoods", typeof(PropertiesOfGoods)), Aggregated]
		public XPCollection<PropertiesOfGoods> PropertiesOfGoodsCollection { get { return GetCollection<PropertiesOfGoods>("PropertiesOfGoodsCollection"); } }
	}

}
