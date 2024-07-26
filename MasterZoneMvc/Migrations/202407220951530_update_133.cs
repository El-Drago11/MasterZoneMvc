namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_133 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApartmentAreas", "SubTitle", c => c.String());
            AddColumn("dbo.ApartmentAreas", "ApartmentImage", c => c.String());
            AddColumn("dbo.ApartmentAreas", "Description", c => c.String());
            AddColumn("dbo.ApartmentAreas", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApartmentAreas", "Price");
            DropColumn("dbo.ApartmentAreas", "Description");
            DropColumn("dbo.ApartmentAreas", "ApartmentImage");
            DropColumn("dbo.ApartmentAreas", "SubTitle");
        }
    }
}
