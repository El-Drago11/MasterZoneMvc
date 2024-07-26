namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_33 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Country", c => c.String());
            AddColumn("dbo.Events", "State", c => c.String());
            AddColumn("dbo.Events", "City", c => c.String());
            AddColumn("dbo.Events", "Address", c => c.String());
            AddColumn("dbo.Events", "LandMark", c => c.String());
            AddColumn("dbo.Events", "Pincode", c => c.String());
            AddColumn("dbo.Events", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Events", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "Longitude");
            DropColumn("dbo.Events", "Latitude");
            DropColumn("dbo.Events", "Pincode");
            DropColumn("dbo.Events", "LandMark");
            DropColumn("dbo.Events", "Address");
            DropColumn("dbo.Events", "City");
            DropColumn("dbo.Events", "State");
            DropColumn("dbo.Events", "Country");
        }
    }
}
