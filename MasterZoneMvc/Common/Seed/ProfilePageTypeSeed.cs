using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common.Seed
{
    public class ProfilePageTypeSeed
    {
        private MasterZoneMvc.DAL.MasterZoneDbContext context;

        public ProfilePageTypeSeed(MasterZoneMvc.DAL.MasterZoneDbContext _context)
        {
            context = _context;
        }

        public void SeedData()
        {
            context.ProfilePageTypes.AddOrUpdate(x => x.Id,
                new ProfilePageType() { Id = 1, Key = "gym_web", Name = "GYM", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow},
                new ProfilePageType() { Id = 2, Key = "dance_web", Name = "Dance", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow},
                new ProfilePageType() { Id = 3, Key = "music_web", Name = "Music", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 4, Key = "yoga_web", Name = "Yoga", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 5, Key = "education_web", Name = "Education", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 6, Key = "teacher", Name = "Teacher", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 7, Key = "instructor", Name = "Instructor", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 8, Key = "other", Name = "Other", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 9, Key = "sports", Name = "Sports", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 10, Key = "event_organisation", Name = "EventOrganisation", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 11, Key = "classic_dance", Name = "ClassicDance", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 12, Key = "masterpro", Name = "MasterPro", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow },
                new ProfilePageType() { Id = 13, Key = "master_profile_api", Name = "MasterProfileAPI", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow }
                );
        }
    }
}