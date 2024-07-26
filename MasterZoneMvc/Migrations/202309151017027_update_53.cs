namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_53 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        KeyName = c.String(),
                        TextValue = c.String(),
                        IsActive = c.Int(nullable: false),
                        IsLimited = c.Int(nullable: false),
                        PanelTypeId = c.Long(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemFeatures",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ItemId = c.Long(nullable: false),
                        ItemType = c.String(),
                        FeatureId = c.Long(nullable: false),
                        IsLimited = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Licenses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CertificateId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        LicenseLogo = c.String(),
                        CertificateImage = c.String(),
                        SignatureImage = c.String(),
                        IsPaid = c.Int(nullable: false),
                        CommissionType = c.String(),
                        CommissionValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AchievingOrder = c.Int(nullable: false),
                        TimePeriod = c.String(),
                        LicensePermissions = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserLoginFeatures",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        FeatureId = c.Long(nullable: false),
                        IsLimited = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        AddOnLimit = c.Int(nullable: false),
                        IsActive = c.Int(nullable: false),
                        Comments = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Certificates", "ProfilePageTypeId", c => c.Long(nullable: false));
            DropColumn("dbo.Certificates", "AdditionalInformation");
            DropColumn("dbo.Certificates", "Price");
            DropColumn("dbo.Certificates", "CertificatePermissions");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Certificates", "CertificatePermissions", c => c.String());
            AddColumn("dbo.Certificates", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Certificates", "AdditionalInformation", c => c.String());
            DropColumn("dbo.Certificates", "ProfilePageTypeId");
            DropTable("dbo.UserLoginFeatures");
            DropTable("dbo.Licenses");
            DropTable("dbo.ItemFeatures");
            DropTable("dbo.Features");
        }
    }
}
