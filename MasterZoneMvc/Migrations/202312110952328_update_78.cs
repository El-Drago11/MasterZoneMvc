namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_78 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingBookings", "LicenseId", c => c.Long(nullable: false));
            AddColumn("dbo.TrainingBookings", "LicenseBookingId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingBookings", "LicenseBookingId");
            DropColumn("dbo.TrainingBookings", "LicenseId");
        }
    }
}
