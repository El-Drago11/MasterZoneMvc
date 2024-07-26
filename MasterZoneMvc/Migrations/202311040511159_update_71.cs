namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_71 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApartmentBookings", "ApartmentAreaId", c => c.Long(nullable: false));
            AddColumn("dbo.ApartmentBookings", "ApartmentBlockId", c => c.Long(nullable: false));
            DropColumn("dbo.ApartmentBookings", "BlockName");
            DropColumn("dbo.ApartmentBookings", "AreaName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApartmentBookings", "AreaName", c => c.String());
            AddColumn("dbo.ApartmentBookings", "BlockName", c => c.String());
            DropColumn("dbo.ApartmentBookings", "ApartmentBlockId");
            DropColumn("dbo.ApartmentBookings", "ApartmentAreaId");
        }
    }
}
