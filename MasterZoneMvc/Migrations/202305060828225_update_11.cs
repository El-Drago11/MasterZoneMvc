namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "AdvertisementCategory", c => c.String());
            AddColumn("dbo.Advertisements", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Advertisements", "ImageAspectRatio", c => c.String());
            AddColumn("dbo.Advertisements", "ImageResolution", c => c.String());
            AddColumn("dbo.Advertisements", "ImageOrientationType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advertisements", "ImageOrientationType");
            DropColumn("dbo.Advertisements", "ImageResolution");
            DropColumn("dbo.Advertisements", "ImageAspectRatio");
            DropColumn("dbo.Advertisements", "Status");
            DropColumn("dbo.Advertisements", "AdvertisementCategory");
        }
    }
}
