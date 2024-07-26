namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "CreatedForLoginId", c => c.Long(nullable: false));
            DropColumn("dbo.Advertisements", "ImageAspectRatio");
            DropColumn("dbo.Advertisements", "ImageResolution");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advertisements", "ImageResolution", c => c.String());
            AddColumn("dbo.Advertisements", "ImageAspectRatio", c => c.String());
            DropColumn("dbo.Advertisements", "CreatedForLoginId");
        }
    }
}
