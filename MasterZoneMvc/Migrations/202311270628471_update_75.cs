namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_75 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trainings", "LicenseId", c => c.Long(nullable: false));
            AddColumn("dbo.Trainings", "BusinessLicenseId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trainings", "BusinessLicenseId");
            DropColumn("dbo.Trainings", "LicenseId");
        }
    }
}
