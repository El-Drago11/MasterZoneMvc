namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_83 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransferPackages", "PlanBookingId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransferPackages", "PlanBookingId");
        }
    }
}
