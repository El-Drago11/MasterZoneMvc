namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_27 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "SortOrder", c => c.Int(nullable: false));
            AddColumn("dbo.Menus", "IsShowOnHomePage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "IsShowOnHomePage");
            DropColumn("dbo.Menus", "SortOrder");
        }
    }
}
