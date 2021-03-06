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

    [Persistent(@"Table_121")]
    public partial class DocInvoiceOrderGoods : XPBaseObject
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
        SUTZ_2.Module.BO.References.Delimeters fDelimeter;
        [Persistent(@"f003")]
        [DevExpress.Xpo.DisplayName(@"Разделитель")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public SUTZ_2.Module.BO.References.Delimeters Delimeter
        {
            get { return fDelimeter; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Delimeters>("Delimeter", ref fDelimeter, value); }
        }
        DocInvoiceOrder fParentID;
        [Persistent(@"f004")]
        [Association(@"DocListOfGoodsInvoiceOrderReferencesDocInvoiceOrder")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public DocInvoiceOrder ParentID
        {
            get { return fParentID; }
            set { SetPropertyValue<DocInvoiceOrder>("ParentID", ref fParentID, value); }
        }
        SUTZ_2.Module.BO.References.Goods fGood;
        [Persistent(@"f005")]
        [DevExpress.Xpo.DisplayName(@"Товар")]
        public SUTZ_2.Module.BO.References.Goods Good
        {
            get { return fGood; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Goods>("Good", ref fGood, value); }
        }
        decimal fCoeff;
        /// <summary>
        /// Количество коробок
        /// </summary>
        [Persistent(@"f006")]
        [DevExpress.Xpo.DisplayName(@"Коэфф")]
        public decimal Coeff
        {
            get { return fCoeff; }
            set { SetPropertyValue<decimal>("Coeff", ref fCoeff, value); }
        }
        SUTZ_2.Module.BO.References.Units fUnit;
        [Persistent(@"f007")]
        [DevExpress.Xpo.DisplayName(@"Единица")]
        public SUTZ_2.Module.BO.References.Units Unit
        {
            get { return fUnit; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Units>("Unit", ref fUnit, value); }
        }
        decimal fQuantityOfUnits;
        /// <summary>
        /// Количество коробок
        /// </summary>
        [Persistent(@"f008")]
        [DevExpress.Xpo.DisplayName(@"Кол.кор")]
        public decimal QuantityOfUnits
        {
            get { return fQuantityOfUnits; }
            set { SetPropertyValue<decimal>("QuantityOfUnits", ref fQuantityOfUnits, value); }
        }
        decimal fQuantityOfItems;
        /// <summary>
        /// Количество штук
        /// </summary>
        [Persistent(@"f009")]
        [DevExpress.Xpo.DisplayName(@"Кол.шт")]
        public decimal QuantityOfItems
        {
            get { return fQuantityOfItems; }
            set { SetPropertyValue<decimal>("QuantityOfItems", ref fQuantityOfItems, value); }
        }
        decimal fTotalQuantity;
        /// <summary>
        /// Итого, количество:
        /// </summary>
        [Persistent(@"f010")]
        [DevExpress.Xpo.DisplayName(@"Итого")]
        public decimal TotalQuantity
        {
            get { return fTotalQuantity; }
            set { SetPropertyValue<decimal>("TotalQuantity", ref fTotalQuantity, value); }
        }
        DateTime fBestBefore;
        /// <summary>
        /// Срок годности
        /// </summary>
        [Persistent(@"f011")]
        [DevExpress.Xpo.DisplayName(@"Срок годности")]
        public DateTime BestBefore
        {
            get { return fBestBefore; }
            set { SetPropertyValue<DateTime>("BestBefore", ref fBestBefore, value); }
        }
        SUTZ_2.Module.BO.References.PropertiesOfGoods fProperty;
        [Persistent(@"f012")]
        [DevExpress.Xpo.DisplayName(@"Характеристика")]
        public SUTZ_2.Module.BO.References.PropertiesOfGoods Property
        {
            get { return fProperty; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.PropertiesOfGoods>("Property", ref fProperty, value); }
        }
        [Persistent(@"f013")]
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
        [Persistent(@"f014")]
        [DevExpress.Xpo.DisplayName(@"Пометка удаления")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public bool IsMarkDeleted
        {
            get { return fIsMarkDeleted; }
            set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
        }
    }

}
