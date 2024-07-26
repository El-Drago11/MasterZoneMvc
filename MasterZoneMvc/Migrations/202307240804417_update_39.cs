namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_39 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessTimings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        DayName = c.String(),
                        DayValue = c.Int(nullable: false),
                        IsOpened = c.Int(nullable: false),
                        OpeningTime_12HoursFormat = c.String(),
                        OpeningTime_24HoursFormat = c.String(),
                        ClosingTime_12HoursFormat = c.String(),
                        ClosingTime_24HoursFormat = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Trainings", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
            AddColumn("dbo.Trainings", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
            AlterColumn("dbo.Classes", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
            AlterColumn("dbo.Classes", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
            AlterColumn("dbo.Events", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
            AlterColumn("dbo.Events", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 15));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Events", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Classes", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Classes", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Trainings", "Longitude");
            DropColumn("dbo.Trainings", "Latitude");
            DropTable("dbo.BusinessTimings");
        }
    }
}
