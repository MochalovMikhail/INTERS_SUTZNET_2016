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

		/// <summary>
		/// Таблица заданий для моб.сутз - основная таблица
		/// </summary>
	[Persistent(@"Table_029")]
		[System.ComponentModel.DefaultProperty("Description")]
	public partial class MobileWorkUnit : XPBaseObject {
		[Key(true)]
		[Persistent(@"f001")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		int fidd;
		[PersistentAlias("fidd")]
		public int idd {
			get { return fidd; }
		}
		string fBarcode;
		/// <summary>
		/// Штрих-код паллета (если он есть), отсканированный в процессе выполнения юнита задания
		/// </summary>
		[Persistent(@"f002")]
		[DisplayName(@"Штрихкод")]
		public string Barcode {
			get { return fBarcode; }
			set { SetPropertyValue<string>("Barcode", ref fBarcode, value); }
		}
		SUTZ_2.Module.BO.References.Delimeters fDelimeter;
		[Persistent(@"f003")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Разделитель")]
		public SUTZ_2.Module.BO.References.Delimeters Delimeter {
			get { return fDelimeter; }
			set { SetPropertyValue<SUTZ_2.Module.BO.References.Delimeters>("Delimeter", ref fDelimeter, value); }
		}
		SUTZ_2.Module.BO.References.JobTypes fJobType;
		/// <summary>
		/// Вид работы, присвоенный единице работы
		/// </summary>
		[Persistent(@"f004")]
		[DisplayName(@"Вид работы")]
		public SUTZ_2.Module.BO.References.JobTypes JobType {
			get { return fJobType; }
			set { SetPropertyValue<SUTZ_2.Module.BO.References.JobTypes>("JobType", ref fJobType, value); }
		}
		SUTZ_2.Module.BO.References.Users fAssignedUser;
		/// <summary>
		/// Пользователь, которому назначено это задание. Может быть пустым, тогда задание доступно всем. Если пользователь указан, задание доступно только ему.
		/// </summary>
		[Persistent(@"f005")]
		[DisplayName(@"Пользователь")]
		public SUTZ_2.Module.BO.References.Users AssignedUser {
			get { return fAssignedUser; }
			set { SetPropertyValue<SUTZ_2.Module.BO.References.Users>("AssignedUser", ref fAssignedUser, value); }
		}
		SUTZ_2.Module.BO.enTypesOfMobileStates fTypeOfMobileState;
		/// <summary>
		/// Состояние задания - Доступен для работы, не доступен, Выполнен и т.д.
		/// </summary>
		[Persistent(@"f006")]
		[DisplayName(@"Состояние задания")]
		public SUTZ_2.Module.BO.enTypesOfMobileStates TypeOfMobileState {
			get { return fTypeOfMobileState; }
			set { SetPropertyValue<SUTZ_2.Module.BO.enTypesOfMobileStates>("TypeOfMobileState", ref fTypeOfMobileState, value); }
		}
		SUTZ_2.Module.BO.References.Users fSendedUser;
		/// <summary>
		/// Пользователь, который отправил задание в работу
		/// </summary>
		[Persistent(@"f007")]
		[DisplayName(@"Назначено пользователем")]
		public SUTZ_2.Module.BO.References.Users SendedUser {
			get { return fSendedUser; }
			set { SetPropertyValue<SUTZ_2.Module.BO.References.Users>("SendedUser", ref fSendedUser, value); }
		}
		DateTime fdTimeInsertUnit;
		/// <summary>
		/// Дата-время добавления пользователем задания в таблицу
		/// </summary>
		[Persistent(@"f008")]
		[DisplayName(@"Время создания задания")]
		public DateTime dTimeInsertUnit {
			get { return fdTimeInsertUnit; }
			set { SetPropertyValue<DateTime>("dTimeInsertUnit", ref fdTimeInsertUnit, value); }
		}
		[Persistent(@"f009")]
		[DisplayName(@"Комментарий")]
		[Delayed(false)]
		public string Comment {
			get { return GetDelayedPropertyValue<string>("Comment"); }
			set { SetDelayedPropertyValue<string>("Comment", value); }
		}
		DateTime fdTimeProductionFact;
		/// <summary>
		/// Дата-время фактического товара
		/// </summary>
		[Persistent(@"f010")]
		[DisplayName(@"Факт дата товара")]
		public DateTime dTimeProductionFact {
			get { return fdTimeProductionFact; }
			set { SetPropertyValue<DateTime>("dTimeProductionFact", ref fdTimeProductionFact, value); }
		}
		decimal fQuantityFact;
		[Persistent(@"f011")]
		[DisplayName(@"Количество факт.")]
		public decimal QuantityFact {
			get { return fQuantityFact; }
			set { SetPropertyValue<decimal>("QuantityFact", ref fQuantityFact, value); }
		}
		SUTZ_2.Module.BO.enTypesOfUnitWork fTypeOfUnitWork;
		/// <summary>
		/// Сохраняет состояние текущего этапа внутри единицы задания - отсканирован взять из или ПоложитьВ и т.д.
		/// </summary>
		[Persistent(@"f012")]
		[DisplayName(@"Текущий этап выполнения задания")]
		public SUTZ_2.Module.BO.enTypesOfUnitWork TypeOfUnitWork {
			get { return fTypeOfUnitWork; }
			set { SetPropertyValue<SUTZ_2.Module.BO.enTypesOfUnitWork>("TypeOfUnitWork", ref fTypeOfUnitWork, value); }
		}
		Guid fidGUID;
		[Persistent(@"f013")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"IDD SUTZ")]
		public Guid idGUID {
			get { return fidGUID; }
			set { SetPropertyValue<Guid>("idGUID", ref fidGUID, value); }
		}
		SUTZ_2.Module.BO.Documents.BaseDocument fBaseDoc;
		[Persistent(@"f014")]
		[DisplayName(@"Документ")]
		public SUTZ_2.Module.BO.Documents.BaseDocument BaseDoc {
			get { return fBaseDoc; }
			set { SetPropertyValue<SUTZ_2.Module.BO.Documents.BaseDocument>("BaseDoc", ref fBaseDoc, value); }
		}
		bool fIsMarkDeleted;
		[Persistent(@"f015")]
		[DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
		[DisplayName(@"Пометка удаления")]
		public bool IsMarkDeleted {
			get { return fIsMarkDeleted; }
			set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
		}
		decimal fQuantityPlan;
		[Persistent(@"f016")]
		[DisplayName(@"Количество план")]
		public decimal QuantityPlan {
			get { return fQuantityPlan; }
			set { SetPropertyValue<decimal>("QuantityPlan", ref fQuantityPlan, value); }
		}
		[Association(@"MobileWorkDocRowsReferencesMobileWorkUnit", typeof(MobileWorkDocRows)), Aggregated]
		public XPCollection<MobileWorkDocRows> MobileWorkDocRowsCollection { get { return GetCollection<MobileWorkDocRows>("MobileWorkDocRowsCollection"); } }
		[Association(@"MobileWorkUsersReferencesMobileWorkUnit", typeof(MobileWorkUsers)), Aggregated]
		public XPCollection<MobileWorkUsers> MobileWorkUsersCollection { get { return GetCollection<MobileWorkUsers>("MobileWorkUsersCollection"); } }
	}

}
