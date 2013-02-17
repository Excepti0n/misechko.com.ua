using System.Collections.Generic;
using System.Data.SqlTypes;
using misechko.com.data.Entities;
using misechko.com.data.Utils;

namespace misechko.com.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class AutomaticMigrationConfiguration : DbMigrationsConfiguration<misechko.com.data.EF.MPDataContext>
    {
        public AutomaticMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(misechko.com.data.EF.MPDataContext context)
        {
            var roles = new List<UserRole>
                {
                    new UserRole
                        {
                            Id = Guid.Parse("9727d3e4-0269-46e1-ad7c-bfbdc9c074bc"),
                            AdminFeaturesAvailable = true,
                            Description = "Администратор сайту Місечко та Партнери",
                            RoleName = "Administrator"
                        }
                };

            foreach (var userRole in roles.Where(userRole => !context.UserRoles.Any(rl => rl.RoleName == userRole.RoleName)))
            {
                context.UserRoles.Add(userRole);
            }

            var adminUser = new SiteUser()
            {
                Id = Guid.Parse("479460f6-06c1-43fb-96c3-6ff161255c04"),
                CreateDate = DateTime.Now,
                UserName = "VM",
                Password = Crypto.HashPassword("m83inject"),
                UserRoles = context.UserRoles.Local.Where(rl => rl.RoleName == "Administrator").ToList(),
                IsLockedOut = false,
                LastActivityDate = SqlDateTime.MinValue.Value,
                LastLoginDate = SqlDateTime.MinValue.Value,
                LastLockoutDate = SqlDateTime.MinValue.Value,
                LastPasswordChangedDate = SqlDateTime.MinValue.Value,
                LastPasswordFailureDate = SqlDateTime.MinValue.Value,
                PasswordVerificationTokenExpirationDate = SqlDateTime.MinValue.Value
            };


            if (!context.SiteUsers.Any(usr => usr.UserName == adminUser.UserName)) context.SiteUsers.Add(adminUser);

            base.Seed(context);
        }
    }
}
