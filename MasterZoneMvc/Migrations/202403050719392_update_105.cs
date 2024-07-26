namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_105 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MainPlanBookings", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MainPlanBookings", "Status");
        }
    }
}
