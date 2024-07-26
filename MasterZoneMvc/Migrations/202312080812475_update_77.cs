namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_77 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trainings", "LicenseBookingId", c => c.Long(nullable: false));
            DropColumn("dbo.Trainings", "BusinessLicenseId");
            DropTable("dbo.BusinessLicenses");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BusinessLicenses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LicenseBookingId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        LicenseId = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                        QuantityUsed = c.Int(nullable: false),
                        LicenseIsPaid = c.Int(nullable: false),
                        LicensePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseCommissionType = c.String(),
                        LicenseCommissionValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseGSTPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseGSTDescription = c.String(),
                        LicenseMinSellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Trainings", "BusinessLicenseId", c => c.Long(nullable: false));
            DropColumn("dbo.Trainings", "LicenseBookingId");
        }
    }
}
