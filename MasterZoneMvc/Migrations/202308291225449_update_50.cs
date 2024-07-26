namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_50 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassBookings", "BatchId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClassBookings", "BatchId");
        }
    }
}
