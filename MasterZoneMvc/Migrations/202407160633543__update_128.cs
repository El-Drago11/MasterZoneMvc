namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_128 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SportsBookingCheaqueDetails", "UserLoginId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SportsBookingCheaqueDetails", "UserLoginId");
        }
    }
}
