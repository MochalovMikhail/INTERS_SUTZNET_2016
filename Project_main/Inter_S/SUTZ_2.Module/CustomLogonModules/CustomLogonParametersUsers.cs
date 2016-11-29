using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Model;

namespace SUTZ_2.Module.BO.References
{
    [NonPersistent, ModelDefault("Caption","Log on")]
    public class CustomLogonParametersUsers: ICustomObjectSerialize, INotifyPropertyChanged
    {
        
        #region Члены INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        private Users user;
        // свойство текущий пользователь
        [DataSourceProperty("AvailableUsers"), ImmediatePostData]
        public Users User
        {
            get { return user; }
            set
            {
                if (user == value) return;
                user = value;
                if (user != null)
                {
                    UserName = user.UserName;
                }
                else UserName = string.Empty;
                RaisePropertyChanged("User");
            }
        }

        // свойство пароль пользователя
        private string password;
        [PasswordPropertyText(true)]
        public string Password
        {
            get { return password; }
            set
            {
                if (password == value) return;
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        // имя пользователя
        [Browsable(false)]
        public string UserName { get; set; }
 
        private IObjectSpace objectSpace;
        private XPCollection<Users> availableUsers;
        [Browsable(false)]
        public IObjectSpace ObjectSpace
        {
            get { return objectSpace; }
            set { objectSpace = value; }
        }

        [Browsable(false)]
        [CollectionOperationSet(AllowAdd = false)]
        public XPCollection<Users> AvailableUsers
        {
            get
            {
                if (availableUsers == null)
                {
                    availableUsers = ObjectSpace.GetObjects<Users>() as XPCollection<Users>;
                    RefreshAvailableUsers();
                }
                return availableUsers;
            }
        }

        public void Reset()
        {
            ObjectSpace = null;
            availableUsers = null;
            User = null;
            Password = null;
        }

        private void RefreshAvailableUsers()
        {
            if (availableUsers == null)
            {
                return;
            }
        }

         #region Члены ICustomObjectSerialize        
        public void ReadPropertyValues(SettingsStorage storage) {
            User = objectSpace.FindObject<Users>(
                new BinaryOperator("UserName", storage.LoadOption("", "UserName")));
            //if (UsersAuthStd != null) Company = UsersAuthStd.Company;
        }

         public void WritePropertyValues(SettingsStorage storage) {
            storage.SaveOption("", "UserName", UserName);
        }

        #endregion 
    }
}
