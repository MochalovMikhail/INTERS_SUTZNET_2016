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

    [Persistent(@"Table_100")]
    public partial class BaseDocument : XPBaseObject
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
        string fDocNo;
        [Indexed(Name = @"Index_DocNo")]
        [Size(20)]
        [Persistent(@"f002")]
        [DevExpress.Xpo.DisplayName(@"����� ���������")]
        public string DocNo
        {
            get { return fDocNo; }
            set { SetPropertyValue<string>("DocNo", ref fDocNo, value); }
        }
        SUTZ_2.Module.BO.References.Delimeters fDelimeter;
        [Persistent(@"f003")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public SUTZ_2.Module.BO.References.Delimeters Delimeter
        {
            get { return fDelimeter; }
            set { SetPropertyValue<SUTZ_2.Module.BO.References.Delimeters>("Delimeter", ref fDelimeter, value); }
        }
        DateTime fDocDateTime;
        [Persistent(@"f004")]
        [DevExpress.Xpo.DisplayName(@"���� ���������")]
        public DateTime DocDateTime
        {
            get { return fDocDateTime; }
            set { SetPropertyValue<DateTime>("DocDateTime", ref fDocDateTime, value); }
        }
        SUTZ_2.Module.BO.DocStates fDocState;
        /// <summary>
        /// ������ ��������� - �������, ��������, ������.
        /// </summary>
        [Persistent(@"f005")]
        [DevExpress.Xpo.DisplayName(@"������ ���������")]
        public SUTZ_2.Module.BO.DocStates DocState
        {
            get { return fDocState; }
            set { SetPropertyValue<SUTZ_2.Module.BO.DocStates>("DocState", ref fDocState, value); }
        }
        string fComment;
        [Size(SizeAttribute.Unlimited)]
        [Persistent(@"f006")]
        [DevExpress.Xpo.DisplayName(@"�����������")]
        public string Comment
        {
            get { return fComment; }
            set { SetPropertyValue<string>("Comment", ref fComment, value); }
        }
        BaseDocument fDocBase;
        [Persistent(@"f007")]
        [DevExpress.Xpo.DisplayName(@"��������-���������")]
        public BaseDocument DocBase
        {
            get { return fDocBase; }
            set { SetPropertyValue<BaseDocument>("DocBase", ref fDocBase, value); }
        }
        Guid fidGUID;
        [Indexed(Name = @"Index_idGUID", Unique = true)]
        [Persistent(@"f008")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public Guid idGUID
        {
            get { return fidGUID; }
            set { SetPropertyValue<Guid>("idGUID", ref fidGUID, value); }
        }
        SUTZ_2.Module.BO.enDocPropertyStatus fFirstSign;
        /// <summary>
        /// ������ ���� ������� ����������
        /// </summary>
        [Persistent(@"f009")]
        public SUTZ_2.Module.BO.enDocPropertyStatus FirstSign
        {
            get { return fFirstSign; }
            set { SetPropertyValue<SUTZ_2.Module.BO.enDocPropertyStatus>("FirstSign", ref fFirstSign, value); }
        }
        SUTZ_2.Module.BO.enDocPropertyStatus fSecondSign;
        /// <summary>
        /// ������ ���� ����������� ������� ����������
        /// </summary>
        [Persistent(@"f010")]
        public SUTZ_2.Module.BO.enDocPropertyStatus SecondSign
        {
            get { return fSecondSign; }
            set { SetPropertyValue<SUTZ_2.Module.BO.enDocPropertyStatus>("SecondSign", ref fSecondSign, value); }
        }
        SUTZ_2.Module.BO.enDocPropertyStatus fThirdSign;
        /// <summary>
        /// ������ ���� ������� ����������
        /// </summary>
        [Persistent(@"f011")]
        public SUTZ_2.Module.BO.enDocPropertyStatus ThirdSign
        {
            get { return fThirdSign; }
            set { SetPropertyValue<SUTZ_2.Module.BO.enDocPropertyStatus>("ThirdSign", ref fThirdSign, value); }
        }
        SUTZ_2.Module.BO.enDocPropertyStatus fFourthSign;
        /// <summary>
        /// ��������� ���� ������� ����������
        /// </summary>
        [Persistent(@"f012")]
        public SUTZ_2.Module.BO.enDocPropertyStatus FourthSign
        {
            get { return fFourthSign; }
            set { SetPropertyValue<SUTZ_2.Module.BO.enDocPropertyStatus>("FourthSign", ref fFourthSign, value); }
        }
        bool fIsMarkDeleted;
        [Persistent(@"f013")]
        [DevExpress.Xpo.DisplayName(@"������� ��������")]
        [DevExpress.Persistent.Base.VisibleInListView(false), DevExpress.Persistent.Base.VisibleInDetailView(false), DevExpress.Persistent.Base.VisibleInLookupListView(false)]
        public bool IsMarkDeleted
        {
            get { return fIsMarkDeleted; }
            set { SetPropertyValue<bool>("IsMarkDeleted", ref fIsMarkDeleted, value); }
        }
        SUTZ_2.Module.BO.enDocumentType fDocumentType;
        [Indexed(Name = @"Index_DocumentType")]
        [NonPersistent]
        [DevExpress.Xpo.DisplayName(@"��� ���������")]
        public SUTZ_2.Module.BO.enDocumentType DocumentType
        {
            get { return fDocumentType; }
            set { SetPropertyValue<SUTZ_2.Module.BO.enDocumentType>("DocumentType", ref fDocumentType, value); }
        }
        string fSender;
        [NonPersistent]
        [DevExpress.Xpo.DisplayName(@"�����������")]
        public string Sender
        {
            get { return fSender; }
            set { SetPropertyValue<string>("Sender", ref fSender, value); }
        }
        string fRecipient;
        [NonPersistent]
        [DevExpress.Xpo.DisplayName(@"����������")]
        public string Recipient
        {
            get { return fRecipient; }
            set { SetPropertyValue<string>("Recipient", ref fRecipient, value); }
        }
        decimal fQuantity;
        [NonPersistent]
        [DevExpress.Xpo.DisplayName(@"����������")]
        public decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<decimal>("Quantity", ref fQuantity, value); }
        }
    }

}