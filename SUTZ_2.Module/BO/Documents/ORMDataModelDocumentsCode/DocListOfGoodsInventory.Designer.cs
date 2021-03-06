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
using System.Collections.Generic;
using System.ComponentModel;
namespace SUTZ_2.Module.BO.Documents
{

    [Persistent(@"Table_161")]
    public partial class DocInventoryGoods : XPBaseObject
    {
        int fidd;
        [Key(true)]
        [Persistent(@"f001")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public int idd
        {
            get { return fidd; }
            set { SetPropertyValue<int>("idd", ref fidd, value); }
        }
        int fLineNo;
        [Persistent(@"f002")]
        [DevExpress.Xpo.DisplayName(@"Номер строки")]
        public int LineNo
        {
            get { return fLineNo; }
            set { SetPropertyValue<int>("LineNo", ref fLineNo, value); }
        }
        DocInventory fParentID;
        [Persistent(@"f003")]
        [Association(@"DocListOfGoodsInventoryReferencesDocInventory")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public DocInventory ParentID
        {
            get { return fParentID; }
            set { SetPropertyValue<DocInventory>("ParentID", ref fParentID, value); }
        }
        SUTZ_2.Module.BO.References.Delimeters fDelimeter;
        [Persistent(@"f004")]
        [DevExpress.Xpo.DisplayName(@"Разделитель")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public SUTZ_2.Module.BO.References.Delimeters Delimeter
        {
            get { return fDelimeter; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Delimeters>("Delimeter", ref fDelimeter, value); }
        }
        SUTZ_2.Module.BO.References.Goods fGood;
        [Persistent(@"f005")]
        [DevExpress.Xpo.DisplayName(@"Товар")]
        public SUTZ_2.Module.BO.References.Goods Good
        {
            get { return fGood; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Goods>("Good", ref fGood, value); }
        }
        SUTZ_2.Module.BO.References.Goods fGoodFact;
        [Persistent(@"f006")]
        [DevExpress.Xpo.DisplayName(@"Факт.товар")]
        public SUTZ_2.Module.BO.References.Goods GoodFact
        {
            get { return fGoodFact; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Goods>("GoodFact", ref fGoodFact, value); }
        }
        SUTZ_2.Module.BO.References.Units fUnit;
        [Persistent(@"f007")]
        [DevExpress.Xpo.DisplayName(@"Единица")]
        public SUTZ_2.Module.BO.References.Units Unit
        {
            get { return fUnit; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Units>("Unit", ref fUnit, value); }
        }
        SUTZ_2.Module.BO.References.Units fUnitFact;
        [Persistent(@"f008")]
        [DevExpress.Xpo.DisplayName(@"Факт.Единица")]
        public SUTZ_2.Module.BO.References.Units UnitFact
        {
            get { return fUnitFact; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Units>("UnitFact", ref fUnitFact, value); }
        }
        decimal fCoeff;
        /// <summary>
        /// Количество коробок
        /// </summary>
        [Persistent(@"f009")]
        [DevExpress.Xpo.DisplayName(@"Кол.кор")]
        public decimal Coeff
        {
            get { return fCoeff; }
            set { SetPropertyValue<decimal>("Coeff", ref fCoeff, value); }
        }
        decimal fCoeffFact;
        /// <summary>
        /// Количество коробок
        /// </summary>
        [Persistent(@"f010")]
        [DevExpress.Xpo.DisplayName(@"Кол.кор")]
        public decimal CoeffFact
        {
            get { return fCoeffFact; }
            set { SetPropertyValue<decimal>("CoeffFact", ref fCoeffFact, value); }
        }
        decimal fQuantityOfUnits;
        /// <summary>
        /// Количество коробок
        /// </summary>
        [Persistent(@"f011")]
        [DevExpress.Xpo.DisplayName(@"Кол.кор")]
        public decimal QuantityOfUnits
        {
            get { return fQuantityOfUnits; }
            set { SetPropertyValue<decimal>("QuantityOfUnits", ref fQuantityOfUnits, value); }
        }
        decimal fQuantityOfUnitsFact;
        /// <summary>
        /// Количество коробок
        /// </summary>
        [Persistent(@"f012")]
        [DevExpress.Xpo.DisplayName(@"Кол.кор.факт")]
        public decimal QuantityOfUnitsFact
        {
            get { return fQuantityOfUnitsFact; }
            set { SetPropertyValue<decimal>("QuantityOfUnitsFact", ref fQuantityOfUnitsFact, value); }
        }
        decimal fQuantityOfItems;
        /// <summary>
        /// Количество штук
        /// </summary>
        [Persistent(@"f013")]
        [DevExpress.Xpo.DisplayName(@"Кол.штук")]
        public decimal QuantityOfItems
        {
            get { return fQuantityOfItems; }
            set { SetPropertyValue<decimal>("QuantityOfItems", ref fQuantityOfItems, value); }
        }
        decimal fQuantityOfItemsFact;
        /// <summary>
        /// Количество штук фактическое
        /// </summary>
        [Persistent(@"f014")]
        [DevExpress.Xpo.DisplayName(@"Кол.штук факт")]
        public decimal QuantityOfItemsFact
        {
            get { return fQuantityOfItemsFact; }
            set { SetPropertyValue<decimal>("QuantityOfItemsFact", ref fQuantityOfItemsFact, value); }
        }
        decimal fTotalQuantity;
        /// <summary>
        /// Итого, количество:
        /// </summary>
        [Persistent(@"f015")]
        [DevExpress.Xpo.DisplayName(@"Баз.количество")]
        public decimal TotalQuantity
        {
            get { return fTotalQuantity; }
            set { SetPropertyValue<decimal>("TotalQuantity", ref fTotalQuantity, value); }
        }
        decimal fTotalQuantityFact;
        /// <summary>
        /// Итого, количество:
        /// </summary>
        [Persistent(@"f016")]
        [DevExpress.Xpo.DisplayName(@"Баз.количество")]
        public decimal TotalQuantityFact
        {
            get { return fTotalQuantityFact; }
            set { SetPropertyValue<decimal>("TotalQuantityFact", ref fTotalQuantityFact, value); }
        }
        DateTime fBestBefore;
        /// <summary>
        /// Срок годности
        /// </summary>
        [Persistent(@"f017")]
        [DevExpress.Xpo.DisplayName(@"Срок годности")]
        public DateTime BestBefore
        {
            get { return fBestBefore; }
            set { SetPropertyValue<DateTime>("BestBefore", ref fBestBefore, value); }
        }
        DateTime fBestBeforeFact;
        /// <summary>
        /// Срок годности факт
        /// </summary>
        [Persistent(@"f018")]
        [DevExpress.Xpo.DisplayName(@"Факт срок годности")]
        public DateTime BestBeforeFact
        {
            get { return fBestBeforeFact; }
            set { SetPropertyValue<DateTime>("BestBeforeFact", ref fBestBeforeFact, value); }
        }
        SUTZ_2.Module.BO.References.StorageCodes fTakeFrom;
        [Persistent(@"f019")]
        [DevExpress.Xpo.DisplayName(@"Взять из")]
        public SUTZ_2.Module.BO.References.StorageCodes TakeFrom
        {
            get { return fTakeFrom; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.StorageCodes>("TakeFrom", ref fTakeFrom, value); }
        }
        SUTZ_2.Module.BO.References.PropertiesOfGoods fProperty;
        [Persistent(@"f020")]
        [DevExpress.Xpo.DisplayName(@"Характеристика")]
        public SUTZ_2.Module.BO.References.PropertiesOfGoods Property
        {
            get { return fProperty; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.PropertiesOfGoods>("Property", ref fProperty, value); }
        }
        SUTZ_2.Module.BO.References.PropertiesOfGoods fPropertyFact;
        [Persistent(@"f021")]
        [DevExpress.Xpo.DisplayName(@"Характеристика факт")]
        public SUTZ_2.Module.BO.References.PropertiesOfGoods PropertyFact
        {
            get { return fPropertyFact; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.PropertiesOfGoods>("PropertyFact", ref fPropertyFact, value); }
        }
        [Persistent(@"f022")]
        [Delayed(false)]
        [DevExpress.Xpo.DisplayName(@"Line idd")]
        [DevExpress.Persistent.Validation.RuleUniqueValue("", DevExpress.Persistent.Validation.DefaultContexts.Save, CriteriaEvaluationBehavior = DevExpress.Persistent.Validation.CriteriaEvaluationBehavior.BeforeTransaction),
DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public Guid DocLineUID
        {
            get { return GetDelayedPropertyValue<Guid>("DocLineUID"); }
            set { SetDelayedPropertyValue<Guid>("DocLineUID", value); }
        }
        bool fIsMarkDeleted;
        [Persistent(@"f023")]
        [DevExpress.Xpo.DisplayName(@"Пометка удаления")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public bool IsMarkDeleted
        {
            get { return fIsMarkDeleted; }
            set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
        }
    }

}
