using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using SUTZ_2.Module.BO.References;
using SUTZ_2.Module.BO.References.Mobile;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using SUTZ_2.Module.BO.Documents;
using SUTZ_2.Module.BO.Exchange.SUTZ1C_SUTZNET;


namespace SUTZ_2.Module
{
    [NonPersistent] 
    public static class currentSessionSettings
    {
        private static XafApplication objXafApp;
        public static XafApplication ObjXafApp
        {
            get { return objXafApp; }
            set
            {
            	objXafApp = value;
            }
        }

        // основное хранилище настроек работы пользователя, сохраняемое между сеансами;
        private static UserSettings userSetting;
        public static UserSettings UserSetting
        {
            get { return userSetting; }
            set { userSetting = value; }
        }

        // вид работы, с которым работает пользователь
        private static JobTypes currentJobType;
        public static JobTypes CurrentJobType
        {
            get
            {
                XPObjectSpace objectSpace = (XPObjectSpace)ObjXafApp.CreateObjectSpace();
                UserSettings settings = objectSpace.Session.FindObject<UserSettings>(new BinaryOperator("idd", userSetting.idd, BinaryOperatorType.Equal));
                if (settings != null)
                {
                    currentJobType = settings.JobType;
                    return settings.JobType;
                }
                return null;
            }
            set
            {
                Guard.ArgumentNotNull(value, "value");

                if (currentJobType!=value)
                {
                    currentJobType = value;
                    // проверка состояния объекта в БД
                    XPObjectSpace objectSpace = (XPObjectSpace)ObjXafApp.CreateObjectSpace();
                    UserSettings settings = objectSpace.Session.FindObject<UserSettings>(new BinaryOperator("idd", userSetting.idd, BinaryOperatorType.Equal));
                    JobTypes objJobType = objectSpace.Session.FindObject<JobTypes>(new BinaryOperator("idd", value.idd, BinaryOperatorType.Equal));
                    if (settings != null)
                    {
                        settings.JobType = objJobType;
                        settings.Save();
                        objectSpace.CommitChanges();   
                    }
                }                                
            }
        }

        // текущее задание, с которым работает пользователь
        private static MobileWorkUnit currentWorkUnit;
        public static MobileWorkUnit  CurrentWorkUnit
        {
            get
            {
                XPObjectSpace objectSpace = (XPObjectSpace)ObjXafApp.CreateObjectSpace();
                UserSettings userSettings = objectSpace.Session.FindObject<UserSettings>(new BinaryOperator("idd", userSetting.idd, BinaryOperatorType.Equal));
                if (userSettings != null)
                {
                    currentWorkUnit = userSettings.CurrentWorkUnit;
                    return currentWorkUnit;// userSettings.CurrentWorkUnit;
                }
                return null;
            }
            set
            {
                Guard.ArgumentNotNull(value, "value");
                if (currentWorkUnit != value)
                {
                    currentWorkUnit = value;
                    // проверка состояния объекта в БД
                    XPObjectSpace objectSpace = (XPObjectSpace)ObjXafApp.CreateObjectSpace();
                    UserSettings settings = objectSpace.Session.FindObject<UserSettings>(new BinaryOperator("idd", userSetting.idd, BinaryOperatorType.Equal));
                    MobileWorkUnit objWorkUnit = objectSpace.Session.FindObject<MobileWorkUnit>(new BinaryOperator("idd", value.idd, BinaryOperatorType.Equal));
                    if (settings != null)
                    {
                        settings.CurrentWorkUnit = objWorkUnit;
                        settings.Save();
                        objectSpace.CommitChanges();
                    }
                }
            }
        }

        // разделитель по умолчанию для текущего сеанса работы, считывается из н
        private static Delimeters defaultDelimeter_;
        public static Delimeters defaultDelimeter { 
            get{
                return defaultDelimeter_;
            }
            set{
                defaultDelimeter_ = value;
            }
        }

        // идентификатор текущего пользователя и сеанса
        private static SUTZ_NET_HostsUsers currentHostUser;
        public static SUTZ_NET_HostsUsers CurrentHostUser
        {
            get { return currentHostUser; }
            set { currentHostUser = value; }
        }

        private static void detectCurrentUserHost(XPObjectSpace objectSpace)
        {
            string hostName = Environment.MachineName;
            string userName = Environment.UserName;

            XPQuery<SUTZ_NET_HostsUsers> qHostUser = new XPQuery<SUTZ_NET_HostsUsers>(((XPObjectSpace)objectSpace).Session);
            var currentUSER = (from c in qHostUser where c.HostName == hostName && c.UserName == userName select c).FirstOrDefault();
            if (currentUSER == null)
            {
                SUTZ_NET_HostsUsers currentUSERForRec = objectSpace.CreateObject<SUTZ_NET_HostsUsers>();
                currentUSERForRec.HostName = hostName;
                currentUSERForRec.UserName = userName;
                try
                {
                    currentUSERForRec.Save();
                    objectSpace.CommitChanges();
                }

                catch (System.Exception ex)
                {

                }
                CurrentHostUser = currentUSERForRec;
            }
            else
            {
                CurrentHostUser = currentUSER;
            }
        }

        public static void inizializeOnBeginWork()
        {
            if (userSetting!=null)
            {
                return;
            }
            // 2. поиск существующего элемента справочника настроек пользователя:
            Users currentUser = (Users)SecuritySystem.CurrentUser;

            defaultDelimeter = currentUser.DefaultDelimeter;
            
            XPObjectSpace xpObjectSpace = (XPObjectSpace)ObjXafApp.CreateObjectSpace();
            UserSettings userSetting1 = xpObjectSpace.FindObject<UserSettings>(new BinaryOperator("ParentID", currentUser.idd));
            Users tekUser = xpObjectSpace.FindObject<Users>(new BinaryOperator("idd", currentUser.idd));
            if (userSetting1!=null)
            {
                UserSetting = userSetting1;
            }
            // 3. элемент настроек пользователя не найден, создание нового
            else
            {
                UserSettings newSettings = xpObjectSpace.CreateObject<UserSettings>();
                newSettings.ParentID = tekUser;
                newSettings.Description = "Настройки пользователя из Current Settings "+tekUser.UserName+" "+DateTime.Now.ToLocalTime();
                newSettings.Save();

                xpObjectSpace.CommitChanges();
                UserSetting = newSettings;
            }

            // 4. создание и установка текущего сеанса пользователя и хоста:
            detectCurrentUserHost(xpObjectSpace);
        }

        // 1. 
    }
}
