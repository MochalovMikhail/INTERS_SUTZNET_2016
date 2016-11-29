using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Data.Filtering;

namespace SUTZ_2.Module.BO.References
{
    public class CustomAuthentication : AuthenticationBase, IAuthenticationStandard
    {
        private string Who;
        private string Pass;
        private CustomLogonParametersUsers logonParameters;

        public CustomAuthentication(string who, string Password)
            : base()
        {
            Who = who;
            Pass = Password;
        }               
        public CustomAuthentication()
        {
            logonParameters = new CustomLogonParametersUsers();
        }
        public override void Logoff()
        {
            base.Logoff();
            logonParameters = new CustomLogonParametersUsers();
        }
        public override void ClearSecuredLogonParameters()
        {
            logonParameters.Password = "";
            base.ClearSecuredLogonParameters();
        }
        public override object Authenticate(IObjectSpace objectSpace)
        {            
            if (logonParameters==null)
            {
                logonParameters = new CustomLogonParametersUsers();
            }
            CustomLogonParametersUsers customLogonParameters = logonParameters as CustomLogonParametersUsers;

            if (Who.Length>0)
            {
                Users user = objectSpace.FindObject<Users>(new BinaryOperator("UserName", Who, BinaryOperatorType.Equal));
                customLogonParameters.User = user;
                customLogonParameters.Password = Pass;
            }            

            if (customLogonParameters.User == null)
                //throw new ArgumentNullException("Users");
                throw new AuthenticationException(
                  Who, "Пользователь "+Who+" не найден в базе!");
            if (!customLogonParameters.User.ComparePassword(customLogonParameters.Password))
                throw new AuthenticationException(
                    customLogonParameters.User.UserName, "Пароль набран не верно!");
            return objectSpace.GetObject(customLogonParameters.User);
        }
        public override IList<Type> GetBusinessClasses()
        {
            return new Type[] { typeof(CustomLogonParametersUsers) };
        }
        public override bool AskLogonParametersViaUI
        {
            get { return false; }
        }
        public override object LogonParameters
        {
            get { return logonParameters; }
        }
        public override bool IsLogoffEnabled
        {
            get { return true; }
        }
    }
}
