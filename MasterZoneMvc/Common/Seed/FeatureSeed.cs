using MasterZoneMvc.Models;
using System.Data.Entity.Migrations;

namespace MasterZoneMvc.Common.Seed
{
    public class FeatureSeed
    {
        private MasterZoneMvc.DAL.MasterZoneDbContext context;

        public FeatureSeed(MasterZoneMvc.DAL.MasterZoneDbContext _context)
        {
            context = _context;
        }

        public void SeedData()
        {
            // Business Panel Features
            context.Features.AddOrUpdate(x => x.KeyName
                , new Feature() { Id = 1, KeyName = "ManageStudent", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many students can create", TextValue = "Manage Student"}
                , new Feature() { Id = 2, KeyName = "ManageEvent", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many events can create", TextValue = "Manage Event"}
                , new Feature() { Id = 3, KeyName = "ManageTraining", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many trainings can create", TextValue = "Manage Training"}
                , new Feature() { Id = 4, KeyName = "ManagePlans", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many Plan or packages can create", TextValue = "Manage Plans"}
                , new Feature() { Id = 5, KeyName = "ManageStaff", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many staff members can create", TextValue = "Manage Staff"}
                , new Feature() { Id = 6, KeyName = "ManageVideoGallery", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many videos can upload", TextValue = "Manage Video Gallery"}
                , new Feature() { Id = 7, KeyName = "ManageImageGallery", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many images can upload", TextValue = "Manage Image Gallery"}                
                , new Feature() { Id = 8, KeyName = "OnlineClass", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many online classes can create", TextValue = "Online Class"}                
                , new Feature() { Id = 9, KeyName = "OfflineClass", PanelTypeId = StaticResources.PanelType_Business, IsActive = 1, IsLimited = 1, Comments = "How many offlince classes can create", TextValue = "Offline Class"}                
                );
        }
    }
}