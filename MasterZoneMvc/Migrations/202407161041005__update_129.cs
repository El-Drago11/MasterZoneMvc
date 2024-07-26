namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_129 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SportsBookingCheaqueDetails", "Request", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SportsBookingCheaqueDetails", "Request");
        }
    }
}
