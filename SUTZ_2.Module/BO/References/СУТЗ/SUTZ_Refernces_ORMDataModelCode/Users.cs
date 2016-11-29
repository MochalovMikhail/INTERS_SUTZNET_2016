using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Persistent.Base.Security;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using System.Security;

 namespace SUTZ_2.Module.BO.References
 {
    [ImageName("BO_Role")]
    public class UsersRole : SecuritySystemRoleBase //SecuritySystemRoleBase
    {
        public UsersRole(Session session) : base(session)
        {
        }

        [Association("Users-UsersRoles")]
        public XPCollection<Users> Users
        {
            get
            {
                return GetCollection<Users>("Users");
            }
        }

        // перенос функционала из класса RoleBase:
        //private RoleImpl role = new RoleImpl();
        //public PersistentPermission AddPermission(IPermission permission) {
        //    PersistentPermission result = new PersistentPermission(Session, permission);
        //    PersistentPermissions.Add(result);
        //    return result;
        //}

        //[MemberDesignTimeVisibility(false), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorCollectionReturnsNewValueOnEachAccess))]
        //public ReadOnlyCollection<IPermission> Permissions {
        //    get {
        //        IList<IPersistentPermission> result = new List<IPersistentPermission>();
        //        foreach(IPersistentPermission persistentPermission in PersistentPermissions) {
        //            result.Add(persistentPermission);
        //        }
        //        return role.GetPermissions(result);
        //    }
        //}
        //public string Name {
        //    get { return role.Name; }
        //    set {
        //        string oldValue = role.Name;
        //        role.Name = value;
        //        OnChanged("Name", oldValue, role.Name);
        //    }
        //}
        //[Aggregated, Association("Users-PersistentPermissions")]
        //public XPCollection<PersistentPermission> PersistentPermissions {
        //    get { return GetCollection<PersistentPermission>("PersistentPermissions"); }
        //}

    }

    public partial class Users : ISecurityUser, IAuthenticationStandardUser, ISecurityUserWithRoles, IOperationPermissionProvider, IAuthenticationActiveDirectoryUser, IBaseSUTZReferences
    {
        private List<IPermission> permissions;

        public Users() : base(Session.DefaultSession) { }
        public Users(Session session) : base(session)
        {
            permissions = new List<IPermission>();
        }
        public override void AfterConstruction() { base.AfterConstruction(); }
        protected override void OnSaving()
        {
            base.OnSaved();
        }
        
        public String Description
        {
            get { return FullUserName; }
            set { }
        }

        #region Члены ISecurityUser

        //private bool isActive = true;
        //public bool IsActive
        //{
        //    get { return isActive; }
        //    set { SetPropertyValue("IsActive", ref isActive, value); }
        //}

        //private string userName = String.Empty;
        //[RuleRequiredField("Users_UserNameRequired", DefaultContexts.Save)]
        //[RuleUniqueValue("Users_UserNameIsUnique", DefaultContexts.Save, "Пользователь с таким именем уже зарегестрирован в системе!")]
        //public string UserName
        //{
        //    get { return userName; }
        //    set { SetPropertyValue("UserName", ref userName, value); }
        //}
        #endregion

        #region Члены IAuthenticationStandardUser

        //private bool changePasswordOnFirstLogon;
        //public bool ChangePasswordOnFirstLogon
        //{
        //    get { return changePasswordOnFirstLogon; }
        //    set
        //    {
        //        SetPropertyValue("ChangePasswordOnFirstLogon", ref changePasswordOnFirstLogon, value);
        //    }
        //}
        //private string storedPassword;
        //[System.ComponentModel.Browsable(false), DevExpress.Xpo.Size(DevExpress.Xpo.SizeAttribute.Unlimited), DevExpress.Xpo.Persistent, DevExpress.ExpressApp.Security.SecurityBrowsable]
        //protected string StoredPassword
        //{
        //    get { return storedPassword; }
        //    set { storedPassword = value; }
        //}

        public bool ComparePassword(string password)
        {
            return SecurityUserBase.ComparePassword(this.StoredPassword, password);
        }
        public void SetPassword(string password)
        {
            this.StoredPassword = new PasswordCryptographer().GenerateSaltedPassword(password);
            OnChanged("StoredPassword");
        }

        #endregion

        #region Члены ISecurityUserWithRoles
        [Association("Users-UsersRoles")]
        [RuleRequiredField("Users_RoleIsRequired", DefaultContexts.Save, TargetCriteria = "IsActive",
        CustomMessageTemplate = "Активный пользователь должен иметь по крайней мере одну роль!")]
        public XPCollection<UsersRole> UsersRoles
        {
            get
            {
                return GetCollection<UsersRole>("UsersRoles");
            }
        }

        IList<ISecurityRole> ISecurityUserWithRoles.Roles
        {
            get
            {
                IList<ISecurityRole> result = new List<ISecurityRole>();
                foreach (UsersRole role in UsersRoles)
                {
                    result.Add(role);
                }
                return result;
            }
        }

        #endregion

        #region Члены IOperationPermissionProvider

        IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions()
        {
            return new IOperationPermission[0];
        }
        IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren()
        {
            return new EnumerableConverter<IOperationPermissionProvider, UsersRole>(UsersRoles);
        }

        #endregion

        //#region Члены IAuthenticationActiveDirectoryUser

        //string IAuthenticationActiveDirectoryUser.UserName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //#endregion
      
    }
}
