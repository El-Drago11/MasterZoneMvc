namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_54 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessCertifications",
                c => new
                    {
                        CertificateId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.CertificateId, t.BusinessOwnerLoginId });
            
            CreateTable(
                "dbo.BusinessLicenses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LicenseRequestId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        LicenseId = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                        QuantityUsed = c.Int(nullable: false),
                        LicenseIsPaid = c.Int(nullable: false),
                        LicensePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseCommissionType = c.String(),
                        LicenseCommissionValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Certificates", "CertificateTypeKey", c => c.String());
            AddColumn("dbo.Licenses", "Signature2Image", c => c.String());
            AddColumn("dbo.Licenses", "Signature3Image", c => c.String());
            AddColumn("dbo.Licenses", "GSTPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Licenses", "GSTDescription", c => c.String());
            AddColumn("dbo.Licenses", "MinSallingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Licenses", "MinSallingPrice");
            DropColumn("dbo.Licenses", "GSTDescription");
            DropColumn("dbo.Licenses", "GSTPercent");
            DropColumn("dbo.Licenses", "Signature3Image");
            DropColumn("dbo.Licenses", "Signature2Image");
            DropColumn("dbo.Certificates", "CertificateTypeKey");
            DropTable("dbo.BusinessLicenses");
            DropTable("dbo.BusinessCertifications");
        }
    }
}
