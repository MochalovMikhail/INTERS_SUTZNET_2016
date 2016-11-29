using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp;
using System.Collections;
using DevExpress.ExpressApp.Xpo;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO.Documents;
using SUTZ_2.Module.BO.References.Mobile;
using DevExpress.ExpressApp.Utils;
using NLog;
using SUTZ_2.Module.PropertyEditors;


namespace SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET
{
    [NonPersistent]
    public class LogMessages
    {
        [Browsable(false)]
        [Key(true)]
        public int Id;

        [XafDisplayName("Время")]
        private DateTime messageTime;
        public DateTime MessageTime
        {
            get { return messageTime; }
            set { messageTime = value; }
        }        

        [Size(10)]
        [XafDisplayName("Тип сообщения")]
        private string messageType;
        
        [VisibleInListView(true)]
        public string MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }        
                
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("Сообщение")]        
        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }

        public LogMessages()
        {

        }        
    }

    // класс для отображения процесса загрузки по типам:
    [NonPersistent]
    public class LogTypeMessages
    {
        // [Browsable(false)]
        [Key(true)]
        public int Id;

        [XafDisplayName("Тип объекта")]
        [Description("Тип загружаемого объекта")] 
        private Type objType_;
        public Type objType
        {
            get { return objType_; }
            set { objType_ = value; }
        }

        [XafDisplayName("Всего загружено")]
        [Description("Всего загружено записей с начала работы")] 
        private int totalGoodInbound_;
        [VisibleInListView(true)]
        public int totalItemsInbound
        {
            get { return totalGoodInbound_; }
            set { totalGoodInbound_ = value; }
        }

        [XafDisplayName("Осталось загрузить")]
        [Description("Количество не загруженных записей")] 
        private int forInbound_;
        public int forInbound
        {
            get { return forInbound_; }
            set { forInbound_ = value; }
        }

        [XafDisplayName("Загружено")]
        [Description("Количество загруженных записей за сеанс")]
        private int inbounded_;
        public int inbounded
        {
            get { return inbounded_; }
            set { inbounded_ = value; }
        }

        [XafDisplayName("% выполнения")]
        [Description("% выполнения")] 
        private int percentage_;
        [EditorAlias(CustomWinProgressBarEditor.WinProgressBarEditorAlias)]
        public int percentage
        {
            get { return percentage_; }
            set { percentage_ = value; }
        }   
          public LogTypeMessages()
        {

        }  
    }
}

// простанство имен для описания делегатов для работы модуля обмена:
namespace SUTZ_2.Module.BO.Exchange
{
    public delegate void delegateShowMessage(string message);
    public delegate void delegateCalculateItems(Type objType, int rowCounts);
}
