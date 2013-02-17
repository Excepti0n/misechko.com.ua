using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using misechko.com.data.Entities;
using misechko.com.data.Utils;

namespace misechko.com.data.EF
{
    public class MPDataContextInitializer : DropCreateDatabaseAlways<MPDataContext>
    {
        protected override void Seed(MPDataContext context)
        {
            //context.UserRoles.Add(

            //var adminUser = new SiteUser()
            //    {
            //        CreateDate = DateTime.Now,
            //        UserName = "VM", Password = Crypto.HashPassword("m83inject"),
            //        UserRoles = context.UserRoles.Local.Where(rl => rl.RoleName == "Administrator").ToList(),
            //        IsLockedOut = false,
            //        LastActivityDate = SqlDateTime.MinValue.Value,
            //        LastLoginDate = SqlDateTime.MinValue.Value,
            //        LastLockoutDate = SqlDateTime.MinValue.Value,
            //        LastPasswordChangedDate = SqlDateTime.MinValue.Value,
            //        LastPasswordFailureDate = SqlDateTime.MinValue.Value,
            //        PasswordVerificationTokenExpirationDate = SqlDateTime.MinValue.Value,
            //        Email = "vm@misechko.com.ua"
            //    };

            //context.SiteUsers.Add(adminUser);

            //var errors = context.GetValidationErrors();

            base.Seed(context);
        }
    }
}
