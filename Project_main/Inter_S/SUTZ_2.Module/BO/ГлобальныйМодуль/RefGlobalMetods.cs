using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using SUTZ_2.Module.BO.References;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;

namespace SUTZ_2.Module.BO
{
    // Класс используется для расширяющих методов:
    public static class MyExtension
    {
        public static DevExpress.Xpo.Session Session(this IObjectSpace objectSpace)
        {
            Guard.ArgumentNotNull(objectSpace, "objectSpace");

            XPObjectSpace objectSpaceInstance = objectSpace as DevExpress.ExpressApp.Xpo.XPObjectSpace;
            if (objectSpaceInstance == null)
                throw new Exception("Unable to cast IObjectSpace to DevExpress.ExpressApp.Xpo");
            return objectSpaceInstance.Session;
        }
    }

    // класс с глобальными методами:
    public class RefGlobalMetods: IDisposable
    {
        private IObjectSpace objectSpace;
        public IObjectSpace ObjectSpace
        {
            get { return objectSpace; }
            set { objectSpace = value; }
        }

        public Int64 generateNewCodeByClassFullName(XPClassInfo classInfo)
        {
           // 1. get full name for class
            string strClassFullName = classInfo.FullName;
              return 1999;

            //MaxNumbersForObjects refMaxNumber = Session.DefaultSession. FindObject<MaxNumbersForObjects>(new BinaryOperator("idd", strClassFullName,BinaryOperatorType.Equal));
            //if (refMaxNumber==null)
            //{
            //    // create new record
            //    MaxNumbersForObjects newMaxNumber = new MaxNumbersForObjects((Session)objectSpace);
            //    newMaxNumber.Delimeter = null;
            //    newMaxNumber.FullClassName = strClassFullName;
            //    newMaxNumber.idd = strClassFullName;
            //    newMaxNumber.MaxFullCode = "";
            //    newMaxNumber.MaxNumber = 0;
            //    newMaxNumber.Prefix = null;
            //    newMaxNumber.ShortClassName = classInfo.ClassType.Name;
            //    try
            //    {
            //        newMaxNumber.Save();
            //    }
            //    catch (System.Exception ex)
            //    {
                	
            //    }
            //}
            //else
            //{

            //}
            
        }

        // Метод выполняет поиск товара по штрих-коду подчиненного справочника Штрих-коды товара
        public Goods findGoodByEANCode(String strBarcode)
        {
            // 1. поиск элемента справочника Штрихкоды товаров:
            Goods goodFinded = null;
            XPQuery<BarcodesOfGoods> barcodesOfGoods = new XPQuery<BarcodesOfGoods>(ObjectSpace.Session());
            var firstBarcode = (from o in barcodesOfGoods where o.Barcode == strBarcode && o.IsDeleted != true select o).FirstOrDefault();

            if (firstBarcode == null)
            {
                return goodFinded;
            }
            else
            {
                return firstBarcode.ParentId;
            }
        }

        //public void checkAndCreateNewUserSUTZWhenLogon(SecuritySystemUser logonUser)
        //{
        //    String UserName = logonUser.UserName;
        //    Guid userId = logonUser.Oid;

        //    // поиск по идентификатору и создание пользователя
        //    Users tekUser = ObjectSpace.FindObject<Users>(CriteriaOperator.Parse("idd=?", userId));

        //    // 2. create class and call procedure for generate new code:
        //    XPQuery<Users> usersQuery = new XPQuery<Users>(Session.DefaultSession);
        //    var objUser = (from c in usersQuery where (c.idd == logonUser.Oid) select c).First();
        //    if (objUser!=null)
        //    {
        //        return;
        //    }

        //    Users newUser = new Users(Session.DefaultSession);
        //    newUser.idd   = userId;
        //    newUser.Code = generateNewCodeByClassFullName(Session.DefaultSession.GetClassInfo(newUser)).ToString();
        //    newUser.DefaultDelimeter = null;
        //    newUser.Delimeter = null;
        //    newUser.Description = logonUser.UserName;
        //    newUser.Save();
        //}

        #region Члены IDisposable

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (objectSpace != null)
                {
                    objectSpace.Dispose();
                    objectSpace = null;
                }
        }
        ~RefGlobalMetods()
        {
            Dispose(false);
        }          


        #endregion
    }
}
