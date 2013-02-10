using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using misechko.com.data.EF;
using misechko.com.data.Entities;

namespace misechko.com.data.Repositories
{
    public class MPUserRepository : IMPUserRepository
    {
        private MPDataContext _db;

        public MPUserRepository()
        {
            _db = new MPDataContext();
        }

        public IQueryable<SiteUser> GetAllUsers()
        {
            return _db.SiteUsers;
        }

        public SiteUser GetUser(Guid userId)
        {
            return _db.SiteUsers.SingleOrDefault(usr => usr.Id == userId);
        }

        public SiteUser GetUser(string userName)
        {
            return _db.SiteUsers.SingleOrDefault(usr => usr.UserName == userName);
        }

        public int GetNumberOfUsersActiveAfter(DateTime afterWhen)
        {
            return _db.SiteUsers.Where(usr => usr.LastActivityDate > afterWhen).Count();
        }

        public int CountUsersWithName(string nameToMatch)
        {
            return _db.SiteUsers.Where(usr => usr.UserName.Contains(nameToMatch)).Count();
        }

        public IQueryable<SiteUser> UsersWithNamePattern(string nameToMatch)
        {
            return _db.SiteUsers.Where(usr => usr.UserName.Contains(nameToMatch)).OrderBy(usr => usr.UserName);
        }

        public int TotalUsersCount()
        {
            return _db.SiteUsers.Count();
        }

        public IQueryable<SiteUser> GetUsersAsQueryable()
        {
            return _db.SiteUsers.OrderBy(usr => usr.UserName);
        }

        public IQueryable<SiteUser> GetUsersInRole(string roleName)
        {
            return GetUsersInRole(GetRole(roleName));
        }

        public IQueryable<SiteUser> GetUsersInRole(Guid roleId)
        {
            return GetUsersInRole(GetRole(roleId));
        }

        public IQueryable<SiteUser> GetUsersInRole(UserRole role)
        {
            if (!RoleExists(role))
                throw new ArgumentException("MissingRole");

            return _db.UserRoles.SingleOrDefault(rl => rl.Id == role.Id).SiteUsers.AsQueryable();
        }

        public IQueryable<UserRole> GetAllRoles()
        {
            return _db.UserRoles;
        }

        public UserRole GetRole(Guid id)
        {
            return _db.UserRoles.SingleOrDefault(rl => rl.Id == id);
        }

        public UserRole GetRole(string name)
        {
            return _db.UserRoles.SingleOrDefault(rl => rl.RoleName == name);
        }

        public IList<UserRole> GetRolesForUser(string userName)
        {
            return _db.SiteUsers.SingleOrDefault(usr => usr.UserName == userName).UserRoles;
        }

        public IList<UserRole> GetRolesForUser(Guid userId)
        {
            return _db.SiteUsers.SingleOrDefault(usr => usr.Id == userId).UserRoles;
        }

        public IList<UserRole> GetRolesForUser(SiteUser user)
        {
            return GetRolesForUser(user.Id);
        }

        private void AddUser(SiteUser user)
        {
            if (UserExists(user))
                throw new ArgumentException("UserAlreadyExists");

            _db.SiteUsers.Add(user);
        }

        public SiteUser CreateUser(string userName, string password, string email, string displayName)
        {
            var user = CreateUser(userName, password, email);
            user.DisplayName = displayName;

            _db.SaveChanges();

            return user;
        }

        public SiteUser CreateUser(string userName, string password, string email)
        {
            if (string.IsNullOrEmpty(userName.Trim()))
                throw new ArgumentException("The user name provided is invalid. Please check the value and try again.");
            if (string.IsNullOrEmpty(password.Trim()))
                throw new ArgumentException("The password provided is invalid. Please enter a valid password value.");
            if (_db.SiteUsers.Any(user => user.UserName == userName))
                throw new ArgumentException("Username already exists. Please enter a different user name.");

            var newUser = new SiteUser()
                              {
                                  UserName = userName,
                                  Password = password,
                                  Email = email,
                                  CreateDate = DateTime.UtcNow,
                                  IsLockedOut = false,
                                  LastActivityDate = SqlDateTime.MinValue.Value,
                                  LastLoginDate = SqlDateTime.MinValue.Value,
                                  LastLockoutDate = SqlDateTime.MinValue.Value,
                                  LastPasswordChangedDate = SqlDateTime.MinValue.Value,
                                  LastPasswordFailureDate = SqlDateTime.MinValue.Value,
                                  PasswordVerificationTokenExpirationDate = SqlDateTime.MinValue.Value
            };

            try
            {
                AddUser(newUser);
            }
            catch (ArgumentException ae)
            {
                throw ae;
            }
            catch (Exception e)
            {
                throw new ArgumentException("The authentication provider returned an error. Please verify your entry and try again. " +
                    "If the problem persists, please contact your system administrator.");
            }

            _db.SaveChanges();

            return newUser;
        }

        public void DeleteUser(SiteUser user)
        {
            if (!UserExists(user))
                throw new ArgumentException("MissingUser");

            _db.SiteUsers.Remove(user);
        }

        public void DeleteUser(string userName)
        {
            DeleteUser(GetUser(userName));
        }

        public void AddRole(UserRole role)
        {
            if (RoleExists(role))
                throw new ArgumentException("RoleAlreadyExists");

            _db.UserRoles.Add(role);
        }

        public void AddRole(string roleName)
        {
            var role = new UserRole()
            {
                RoleName = roleName
            };

            AddRole(role);
        }

        public void AddRoleToUser(Guid userId, string roleName)
        {
            var usr = _db.SiteUsers.SingleOrDefault(u => u.Id == userId);
            var rl = _db.UserRoles.SingleOrDefault(r => r.RoleName == roleName);
            AddRoleToUser(usr, rl);
        }

        public void AddRoleToUser(string userName, string roleName)
        {
            var usr = _db.SiteUsers.SingleOrDefault(u => u.UserName == userName);
            var rl = _db.UserRoles.SingleOrDefault(r => r.RoleName == roleName);
            AddRoleToUser(usr, rl);
        }

        public void AddRoleToUser(SiteUser user, UserRole role)
        {
            user.UserRoles.Add(role);
        }

        public void DeleteRole(UserRole role)
        {
            if (!RoleExists(role))
                throw new ArgumentException("Role doesn't exist");

            _db.UserRoles.Remove(role);
        }

        public void DeleteRole(string roleName)
        {
            DeleteRole(GetRole(roleName));
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public bool UserExists(SiteUser user)
        {
            if (user == null)
                return false;
 
            return (_db.SiteUsers.SingleOrDefault(u => u.Id == user.Id || u.UserName == user.UserName) != null);
        }

        public bool UserNameTaken(string userName)
        {
            if (userName == String.Empty) return true;

            return (_db.SiteUsers.SingleOrDefault(u => u.UserName == userName) != null);
        }

        public void ClearUserRoles(string userName)
        {
            var user = _db.SiteUsers.Single(vis => vis.UserName == userName);
            user.UserRoles.Clear();
        }

        public bool RoleExists(UserRole role)
        {
            if (role == null)
                return false;

            return (_db.UserRoles.SingleOrDefault(r => r.Id == role.Id || r.RoleName == role.RoleName) != null);
        }
    }
}