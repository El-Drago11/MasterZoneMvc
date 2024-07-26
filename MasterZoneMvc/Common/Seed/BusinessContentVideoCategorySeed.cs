using MasterZoneMvc.Models;
using System;
using System.Data.Entity.Migrations;

namespace MasterZoneMvc.Common.Seed
{
    public class BusinessContentVideoCategorySeed
    {
        private MasterZoneMvc.DAL.MasterZoneDbContext context;

        public BusinessContentVideoCategorySeed(MasterZoneMvc.DAL.MasterZoneDbContext _context)
        {
            context = _context;
        }

        public void SeedData()
        {
            context.BusinessContentVideoCategories.AddOrUpdate(x => x.Id,
                new BusinessContentVideoCategory() { Id = 1, Name = "Fitness", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow, DeletedOn = DateTime.UtcNow, IsDeleted=0, CreatedByLoginId = 0, UpdatedByLoginId = 0},
                new BusinessContentVideoCategory() { Id = 2, Name = "Dance", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow, DeletedOn = DateTime.UtcNow, IsDeleted=0, CreatedByLoginId = 0, UpdatedByLoginId = 0},
                new BusinessContentVideoCategory() { Id = 3, Name = "Yoga", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow, DeletedOn = DateTime.UtcNow, IsDeleted=0, CreatedByLoginId = 0, UpdatedByLoginId = 0},
                new BusinessContentVideoCategory() { Id = 4, Name = "Other", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow, DeletedOn = DateTime.UtcNow, IsDeleted=0, CreatedByLoginId = 0, UpdatedByLoginId = 0}
                //new BusinessContentVideoCategory() { Id = 4, Name = "", IsActive = 1, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow, DeletedOn = DateTime.UtcNow, IsDeleted=0, CreatedByLoginId = 0, UpdatedByLoginId = 0}
                );
        }
    }
}