using MasterZoneMvc.Models;
using System.Data.Entity.Migrations;

namespace MasterZoneMvc.Common.Seed
{
    public class FieldTypeCatalogSeed
    {
        private MasterZoneMvc.DAL.MasterZoneDbContext context;

        public FieldTypeCatalogSeed(MasterZoneMvc.DAL.MasterZoneDbContext _context)
        {
            context = _context;
        }

        public void SeedData()
        {
            // Field Type Catalogs [Dropdowns]
            context.FieldTypeCatalogs.AddOrUpdate(x => x.KeyName
                , new FieldTypeCatalog() { Id = 1, ParentId = 0, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "CertificateTypes", TextValue = "Certificate Type" }
                , new FieldTypeCatalog() { Id = 2, ParentId = 1, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "CT_Events", TextValue = "Events" }
                , new FieldTypeCatalog() { Id = 3, ParentId = 1, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "CT_Diploma", TextValue = "Diploma" }
                , new FieldTypeCatalog() { Id = 4, ParentId = 1, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "CT_MasterPro", TextValue = "Master Pro" }
                , new FieldTypeCatalog() { Id = 5, ParentId = 0, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "LicenseCommissionTypes", TextValue = "License Commission Types" }
                , new FieldTypeCatalog() { Id = 6, ParentId = 5, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "LCT_FixedAmount", TextValue = "Fixed Amount" }
                , new FieldTypeCatalog() { Id = 7, ParentId = 5, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "LCT_PercentageBased", TextValue = "Percentage Based" }
                , new FieldTypeCatalog() { Id = 8, ParentId = 5, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "LCT_Nil", TextValue = "Nil" }
                , new FieldTypeCatalog() { Id = 9, ParentId = 0, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = "FamilyRelationTypes", TextValue = "Family Relation Types" }
                , new FieldTypeCatalog() { Id = 10, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Father_KeyName, TextValue = "Father" }
                , new FieldTypeCatalog() { Id = 11, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Mother_KeyName, TextValue = "Mother" }
                , new FieldTypeCatalog() { Id = 12, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_GrandFather_KeyName, TextValue = "Grand Father" }
                , new FieldTypeCatalog() { Id = 13, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_GrandMother_KeyName, TextValue = "Grand Mother" }
                , new FieldTypeCatalog() { Id = 14, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Husband_KeyName, TextValue = "Husband" }
                , new FieldTypeCatalog() { Id = 15, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Wife_KeyName, TextValue = "Wife" }
                , new FieldTypeCatalog() { Id = 16, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Uncle_KeyName, TextValue = "Uncle" }
                , new FieldTypeCatalog() { Id = 17, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Aunt_KeyName, TextValue = "Aunt" }
                , new FieldTypeCatalog() { Id = 18, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Brother_KeyName, TextValue = "Brother" }
                , new FieldTypeCatalog() { Id = 19, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_SisterInLaw_KeyName, TextValue = "Sister-in-law" }
                , new FieldTypeCatalog() { Id = 20, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Sister_KeyName, TextValue = "Sister" }
                , new FieldTypeCatalog() { Id = 21, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_BrotherInLaw_KeyName, TextValue = "Brother-in-law" }
                , new FieldTypeCatalog() { Id = 22, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Son_KeyName, TextValue = "Son" }
                , new FieldTypeCatalog() { Id = 23, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_DaughterInLaw_KeyName, TextValue = "Daughter-in-law" }
                , new FieldTypeCatalog() { Id = 24, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Daughter_KeyName, TextValue = "Daughter" }
                , new FieldTypeCatalog() { Id = 25, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_SonInLaw_KeyName, TextValue = "Son-in-law" }
                , new FieldTypeCatalog() { Id = 26, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_GrandSon_KeyName, TextValue = "Grand Son" }
                , new FieldTypeCatalog() { Id = 27, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Nephew_KeyName, TextValue = "Nephew" }
                , new FieldTypeCatalog() { Id = 28, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Niece_KeyName, TextValue = "Niece" }
                , new FieldTypeCatalog() { Id = 29, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_GrandDaughter_KeyName, TextValue = "Grand Daughter" }
                , new FieldTypeCatalog() { Id = 30, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_Cousin_KeyName, TextValue = "Cousin" }
                , new FieldTypeCatalog() { Id = 31, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_FatherInLaw_KeyName, TextValue = "Father-in-law" }
                , new FieldTypeCatalog() { Id = 32, ParentId = 9, PanelTypeId = StaticResources.PanelType_SuperAdmin, IsActive = 1, KeyName = StaticResources.FRT_MotherInLaw_KeyName, TextValue = "Mother-in-law" }
                );
        }
    }
}