namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_57 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LicenseBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        LicenseId = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                        LicenseIsPaid = c.Int(nullable: false),
                        LicensePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseCommissionType = c.String(),
                        LicenseCommissionValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseGSTPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LicenseGSTDescription = c.String(),
                        LicenseMinSellingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LicenseBookings");
        }
    }
}
