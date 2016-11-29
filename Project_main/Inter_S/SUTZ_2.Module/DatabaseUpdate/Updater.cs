using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using SUTZ_2.Module.BO.References;
using NLog;

namespace SUTZ_2.Module.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();     

            #region создание администратора СУТЗ
            logger.Trace("Вход в функцию UpdateDatabaseAfterUpdateSchema()");

           UsersRole adminUserRole = ObjectSpace.FindObject<UsersRole>(new BinaryOperator("Name", SecurityStrategy.AdministratorRoleName));
            if (adminUserRole == null)
            {
                adminUserRole = ObjectSpace.CreateObject<UsersRole>();
                adminUserRole.Name = SecurityStrategy.AdministratorRoleName;
                adminUserRole.IsAdministrative = true;
                adminUserRole.CanEditModel = true;
                adminUserRole.Save();
                logger.Trace("UpdateDatabaseAfterUpdateSchema. Создание административной роли.");
            }

            // 2. установка разделителя по умолчанию, и создание нового разделителя, если его еще нет.
            Delimeters defaultDelimeter = ObjectSpace.FindObject<Delimeters>(new BinaryOperator("DefaultDelimeter", true));
           // 2.2 если разделитель не найден,  создание нового:
           if (defaultDelimeter == null)
           {
               defaultDelimeter = ObjectSpace.CreateObject<Delimeters>();
               defaultDelimeter.DefaultDelimeter = true;
               defaultDelimeter.DelimeterID = Guid.NewGuid();
               defaultDelimeter.Description = "Основной разделитель учета";
               defaultDelimeter.Save();
           }

            Users adminUser = ObjectSpace.FindObject<Users>(new BinaryOperator("UserName", "Администратор"));
            if (adminUser == null)
            {
                adminUser = ObjectSpace.CreateObject<Users>();
                adminUser.UserName = "Администратор";
                adminUser.IsActive = true;
                //adminUser.SetPassword("");
                adminUser.UsersRoles.Add(adminUserRole);
                adminUser.DefaultDelimeter = defaultDelimeter;
                adminUser.idGUID = new Guid();
                adminUser.SetPassword("");
                adminUser.ChangePasswordOnFirstLogon = true;
                adminUser.Save();
                logger.Trace("UpdateDatabaseAfterUpdateSchema.Создание пользователя Администратор");                
            }
            ObjectSpace.CommitChanges();
           #endregion
        }     
    }
}
