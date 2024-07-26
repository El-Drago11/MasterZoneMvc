namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_96 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "EventCategoryId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "EventCategoryId");
        }
    }
}
