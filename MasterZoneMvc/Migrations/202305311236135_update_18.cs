namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Classes", "BusinessOwnerLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "InstructorLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "StudentMaxStrength", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "ClassMode", c => c.String());
            AddColumn("dbo.Classes", "ScheduledStartOnTime_24HF", c => c.String());
            AddColumn("dbo.Classes", "ScheduledEndOnTime_24HF", c => c.String());
            AddColumn("dbo.Classes", "ClassDays", c => c.String());
            AddColumn("dbo.Classes", "Country", c => c.String());
            AddColumn("dbo.Classes", "State", c => c.String());
            AddColumn("dbo.Classes", "City", c => c.String());
            AddColumn("dbo.Classes", "FullAddressLocation", c => c.String());
            AddColumn("dbo.Classes", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Classes", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Classes", "MaxStrength");
            DropColumn("dbo.Classes", "IsOnlineMode");
            DropColumn("dbo.Classes", "ScheduledOnDate");
            DropColumn("dbo.Classes", "ScheduledOnTime_24HF");
            DropColumn("dbo.Classes", "CountryId");
            DropColumn("dbo.Classes", "StateId");
            DropColumn("dbo.Classes", "CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Classes", "CityId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "StateId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "CountryId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "ScheduledOnTime_24HF", c => c.String());
            AddColumn("dbo.Classes", "ScheduledOnDate", c => c.String());
            AddColumn("dbo.Classes", "IsOnlineMode", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "MaxStrength", c => c.Int(nullable: false));
            DropColumn("dbo.Classes", "Longitude");
            DropColumn("dbo.Classes", "Latitude");
            DropColumn("dbo.Classes", "FullAddressLocation");
            DropColumn("dbo.Classes", "City");
            DropColumn("dbo.Classes", "State");
            DropColumn("dbo.Classes", "Country");
            DropColumn("dbo.Classes", "ClassDays");
            DropColumn("dbo.Classes", "ScheduledEndOnTime_24HF");
            DropColumn("dbo.Classes", "ScheduledStartOnTime_24HF");
            DropColumn("dbo.Classes", "ClassMode");
            DropColumn("dbo.Classes", "StudentMaxStrength");
            DropColumn("dbo.Classes", "InstructorLoginId");
            DropColumn("dbo.Classes", "BusinessOwnerLoginId");
        }
    }
}
