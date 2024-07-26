namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_84 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlanBookings", "IsTransfered", c => c.Int(nullable: false));
            AddColumn("dbo.PlanBookings", "TransferPackageId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlanBookings", "TransferPackageId");
            DropColumn("dbo.PlanBookings", "IsTransfered");
        }
    }
}
