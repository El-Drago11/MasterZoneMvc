namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_127 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SportsBookingCheaqueDetails", "PlayerCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SportsBookingCheaqueDetails", "PlayerCount");
        }
    }
}
