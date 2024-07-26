namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "Country", c => c.String());
            AddColumn("dbo.UserLogins", "State", c => c.String());
            AddColumn("dbo.UserLogins", "City", c => c.String());
            AddColumn("dbo.UserLogins", "Address", c => c.String());
            AddColumn("dbo.UserLogins", "FullAddressLocation", c => c.String());
            AddColumn("dbo.UserLogins", "Pincode", c => c.String());
            AddColumn("dbo.UserLogins", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
            AddColumn("dbo.UserLogins", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "Longitude");
            DropColumn("dbo.UserLogins", "Latitude");
            DropColumn("dbo.UserLogins", "Pincode");
            DropColumn("dbo.UserLogins", "FullAddressLocation");
            DropColumn("dbo.UserLogins", "Address");
            DropColumn("dbo.UserLogins", "City");
            DropColumn("dbo.UserLogins", "State");
            DropColumn("dbo.UserLogins", "Country");
        }
    }
}
