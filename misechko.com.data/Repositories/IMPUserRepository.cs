using System;
using System.Collections.Generic;
using System.Linq;
using misechko.com.data.Entities;

namespace misechko.com.data.Repositories
{
    public interface IMPUserRepository
    {
        IQueryable<SiteUser> GetAllUsers();

        SiteUser GetUser(Guid userId);
        SiteUser GetUser(string userName);

        int GetNumberOfUsersActiveAfter(DateTime afterWhen);

        int CountUsersWithName(string nameToMatch);
        IQueryable<SiteUser> UsersWithNamePattern(string nameToMatch);

        int TotalUsersCount();

        IQueryable<SiteUser> GetUsersAsQueryable();

        IQueryable<SiteUser> GetUsersInRole(string roleName);
        IQueryable<SiteUser> GetUsersInRole(Guid roleId);
        IQueryable<SiteUser> GetUsersInRole(UserRole role);
        IQueryable<UserRole> GetAllRoles();

        UserRole GetRole(Guid id);

        UserRole GetRole(string name);

        IList<UserRole> GetRolesForUser(string userName);
        IList<UserRole> GetRolesForUser(Guid userId);
        IList<UserRole> GetRolesForUser(SiteUser user);

        SiteUser CreateUser(string username, string password, string email);
        SiteUser CreateUser(string username, string password, string email, string displayName);

        void DeleteUser(SiteUser user);
        void DeleteUser(string userName);


        void AddRole(UserRole role);
        void AddRole(string roleName);

        void AddRoleToUser(Guid userId, string roleName);
        void AddRoleToUser(string userName, string roleName);
        void AddRoleToUser(SiteUser user, UserRole role);

        void DeleteRole(UserRole role);
        void DeleteRole(string roleName);

        void SaveChanges();

        bool UserExists(SiteUser user);
        bool RoleExists(UserRole role);
        bool UserNameTaken(string userName);
        void ClearUserRoles(string userName);
    }
}
