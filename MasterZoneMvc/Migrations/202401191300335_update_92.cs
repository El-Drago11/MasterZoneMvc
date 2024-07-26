namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_92 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Certificates", "Link", c => c.String());
            AddColumn("dbo.ClassCategoryTypes", "Description", c => c.String());
            AddColumn("dbo.ClassCategoryTypes", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.HomePageBannerItems", "Text", c => c.String());
            AddColumn("dbo.HomePageBannerItems", "Link", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HomePageBannerItems", "Link");
            DropColumn("dbo.HomePageBannerItems", "Text");
            DropColumn("dbo.ClassCategoryTypes", "ShowOnHomePage");
            DropColumn("dbo.ClassCategoryTypes", "Description");
            DropColumn("dbo.Certificates", "Link");
        }
    }
}
