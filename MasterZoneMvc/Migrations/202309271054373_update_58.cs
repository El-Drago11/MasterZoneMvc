namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_58 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessLicenses", "LicenseBookingId", c => c.Long(nullable: false));
            AddColumn("dbo.BusinessLicenses", "LicenseGSTPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BusinessLicenses", "LicenseGSTDescription", c => c.String());
            AddColumn("dbo.BusinessLicenses", "LicenseMinSellingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.BusinessLicenses", "LicenseRequestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinessLicenses", "LicenseRequestId", c => c.Long(nullable: false));
            DropColumn("dbo.BusinessLicenses", "LicenseMinSellingPrice");
            DropColumn("dbo.BusinessLicenses", "LicenseGSTDescription");
            DropColumn("dbo.BusinessLicenses", "LicenseGSTPercent");
            DropColumn("dbo.BusinessLicenses", "LicenseBookingId");
        }
    }
}
