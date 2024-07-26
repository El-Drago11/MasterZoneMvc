namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_113 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlanBookings", "Repeat_Purchase", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlanBookings", "Repeat_Purchase");
        }
    }
}
