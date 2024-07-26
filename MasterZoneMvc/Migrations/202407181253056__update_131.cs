namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_131 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SportsBookingCheaqueDetails", "SlotId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SportsBookingCheaqueDetails", "SlotId");
        }
    }
}
