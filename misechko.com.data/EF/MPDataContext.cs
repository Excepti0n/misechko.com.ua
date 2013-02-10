﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using misechko.com.data.Entities;
using misechko.com.data.Migrations;

namespace misechko.com.data.EF
{
    public class MPDataContext: DbContext
    {
        
        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Content> ContentElements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tell Code First to ignore PluralizingTableName convention
            // If you keep this convention then the generated tables will have pluralized names.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //set the initializer to migration
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MPDataContext, AutomaticMigrationConfiguration>());
        }

        static MPDataContext()
        {
            //Database.SetInitializer(new MPDataContextInitializer());
        }
    }
}