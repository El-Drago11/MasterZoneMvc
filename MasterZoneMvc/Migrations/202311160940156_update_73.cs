namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_73 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Licenses", "IsLicenseToTeach", c => c.Int(nullable: false));
            AddColumn("dbo.Licenses", "LicenseToTeach_Type", c => c.String());
            AddColumn("dbo.Licenses", "LicenseToTeach_DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Licenses", "LicenseToTeach_DisplayName");
            DropColumn("dbo.Licenses", "LicenseToTeach_Type");
            DropColumn("dbo.Licenses", "IsLicenseToTeach");
        }
    }
}
