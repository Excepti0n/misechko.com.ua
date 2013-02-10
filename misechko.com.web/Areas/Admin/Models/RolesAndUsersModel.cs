using System.Collections.Generic;
using misechko.com.Application.Membership;

namespace misechko.com.Areas.Admin.Models
{
    public class RolesAndUsersModel
    {
        public List<RoleModel> RoleModels { get; set; }
        public List<MPMembershipUser> UsersInFirstRole { get; set; } 
    }

    public class RoleModel
    {
        public string RoleName { get; set; }
        public int RoleUsersCount { get; set; }
        public bool AdminFeaturesAvailable { get; set; }
    }

}