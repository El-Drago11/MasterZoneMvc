namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_94 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "AdvertisementLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advertisements", "AdvertisementLink");
        }
    }
}
