using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;

namespace SUTZ_2.Module.BO.Documents
{
     public partial class BaseDocument : IDisposable
    {

        private Session session_;

        [VisibleInDetailView(false), VisibleInListView(false)]
        public Session session
        {
            get { return session_; }
            set { session_ = value; }
        }

        public virtual String getNameDoc()
        {
            return "Базовый документ";
        }
      
         #region Члены IDisposable

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (session != null)
                {
                    session.Dispose();
                    session = null;
                }
        }
        ~BaseDocument()
        {
            Dispose(false);
        }
        #endregion

        public BaseDocument(Session paramSession):base(paramSession)
        {
            Guard.ArgumentNotNull(paramSession, "paramSession");
            session = paramSession;
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        // общие методы для всех документов:

        // 1. поиск документа по IDD
        //public BaseDocument findByIddSutz(string strIDD)
        //{
        //    Guid guidParsed = new Guid();

        //    if (!Guid.TryParse(strIDD, out guidParsed))
        //        return null;
        //    XPObjectSpace objectSpace = (XPObjectSpace)currentSessionSettings.ObjXafApp.CreateObjectSpace();
        //    BaseDocument findedElement = objectSpace.FindObject<BaseDocument>(new BinaryOperator("idGUID", strIDD, BinaryOperatorType.Equal),true);
        //    return findedElement;
        //}

        // 2. длинное представление документа
        public override string ToString()
        {
            return String.Format("{0} {1} ({2:dd.MM.yyyy})", getNameDoc(), DocNo.Trim(), DocDateTime);// hh:mm:ss.fff tt
        }

        // 3. модуль проведения документа:
        public virtual int checkIn()
        {
            // метод должен выбрасывать мой класс исключения при возникновении любых ошибок проведения документа
            return 1;
        }

    }
}
