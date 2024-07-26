namespace MasterZoneMvc.Migrations
{
    using MasterZoneMvc.Common.Seed;
    using MasterZoneMvc.Models;
    using MasterZoneMvc.Services;
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MasterZoneMvc.DAL.MasterZoneDbContext>
    {

        bool _SeedData = Convert.ToBoolean(ConfigurationManager.AppSettings["SeedData"]);

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MasterZoneMvc.DAL.MasterZoneDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            BusinessContentVideoCategorySeed businessContentVideoCategorySeed = new BusinessContentVideoCategorySeed(context);
            businessContentVideoCategorySeed.SeedData();

            //FeatureSeed featureSeed1 = new FeatureSeed(context);
            //featureSeed1.SeedData();

            FieldTypeCatalogSeed fieldTypeCatalogSeed1 = new FieldTypeCatalogSeed(context);
            fieldTypeCatalogSeed1.SeedData();

            //UserLoginService userLoginService = new UserLoginService(context);
            //var UserLoginsList = context.UserLogins.ToList();
            //foreach (var userLogin in UserLoginsList)
            //{
            //    userLogin.UniqueUserId = userLoginService.GenerateRandomUniqueUserId(userLogin.RoleId.ToString());
            //    context.SaveChanges();
            //}

            ProfilePageTypeSeed profilePageTypeSeed1 = new ProfilePageTypeSeed(context);
            profilePageTypeSeed1.SeedData();

            PermissionSeed permissionSeed1 = new PermissionSeed(context);
            permissionSeed1.SeedData();

            //LastRecordIdDetailSeed lastRecordIdDetailSeed1 = new LastRecordIdDetailSeed(context);
            //lastRecordIdDetailSeed1.SeedData();

            if (_SeedData == false)
            {
                return;
            }

            // else seed the data to the database
            context.Roles.AddOrUpdate(x => x.Id,
                new Role() { Id = 1, Name = "SuperAdmin", IsActive = 1 },
                new Role() { Id = 2, Name = "SubAdmin", IsActive = 1 },
                new Role() { Id = 3, Name = "Student", IsActive = 1 },
                new Role() { Id = 4, Name = "BusinessAdmin", IsActive = 1 },
                new Role() { Id = 5, Name = "Staff", IsActive = 1 }
                );

            context.UserLogins.AddOrUpdate(x => x.Id,
                new UserLogin()
                {
                    Id = 1,
                    UserName = "superadmin",
                    Email = "admin@gmail.com",
                    Password = "PdmMd8VBTcMlWkpttmVQos/z5bhPAyw0I7aMq89pEfE=", // admin123#
                    PhoneNumber = "1234567890",
                    PhoneNumber_CountryCode = "+91",
                    EmailConfirmed = 0,
                    RoleId = 1,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByLoginId = 0,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedByLoginId = 0,
                    IsDeleted = 0,
                    DeletedOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    DeletedByLoginId = 0,
                    Status = 1,
                }
                );


            /* 
            Staff Categories(Main/Departments): 

            Cleaning garbage
            Maintenance 
            Account
            Store (business )incharge
            Security 
            Plumbing 
            Landscaping 
            Recreation
            Production department
            Sound department 
            Video department 
            Technical department 
            Instructor teacher trainer, Master
            Education department 
            Sports department
            Health department
            Others
            */
            context.StaffCategories.AddOrUpdate(x => x.Id, 
                new StaffCategory() { 
                    Id = 1, 
                    Name = "Others", 
                    IsActive = 1,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByLoginId = 0,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedByLoginId = 0,
                    IsDeleted = 0,
                    DeletedOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                },
                new StaffCategory() { 
                    Id = 2, 
                    Name = "Instructor", 
                    IsActive = 1,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByLoginId = 0,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedByLoginId = 0,
                    IsDeleted = 0,
                    DeletedOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                },
                new StaffCategory()
                {
                    Id = 3,
                    Name = "Housekeeping",
                    IsActive = 1,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByLoginId = 0,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedByLoginId = 0,
                    IsDeleted = 0,
                    DeletedOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                },
                new StaffCategory()
                {
                    Id = 4,
                    Name = "Watchman",
                    IsActive = 1,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByLoginId = 0,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedByLoginId = 0,
                    IsDeleted = 0,
                    DeletedOn = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                }
                );

            PermissionSeed permissionSeed = new PermissionSeed(context);
            permissionSeed.SeedData();

            BusinessPlanDurationTypeSeed businessPlanDurationTypeSeed = new BusinessPlanDurationTypeSeed(context);
            businessPlanDurationTypeSeed.SeedData();

            LastRecordIdDetailSeed lastRecordIdDetailSeed = new LastRecordIdDetailSeed(context);
            lastRecordIdDetailSeed.SeedData();

            ProfilePageTypeSeed profilePageTypeSeed = new ProfilePageTypeSeed(context);
            profilePageTypeSeed.SeedData();

            FeatureSeed featureSeed = new FeatureSeed(context);
            featureSeed.SeedData();

            FieldTypeCatalogSeed fieldTypeCatalogSeed = new FieldTypeCatalogSeed(context);
            fieldTypeCatalogSeed.SeedData();
        }
    }
}
