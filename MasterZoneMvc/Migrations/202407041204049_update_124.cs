namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_124 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessTimings", "OpeningTime2_12HoursFormat", c => c.String());
            AddColumn("dbo.BusinessTimings", "OpeningTime2_24HoursFormat", c => c.String());
            AddColumn("dbo.BusinessTimings", "ClosingTime2_12HoursFormat", c => c.String());
            AddColumn("dbo.BusinessTimings", "ClosingTime2_24HoursFormat", c => c.String());
            AddColumn("dbo.BusinessTimings", "TodayOff", c => c.String());
            AddColumn("dbo.BusinessTimings", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessTimings", "Notes");
            DropColumn("dbo.BusinessTimings", "TodayOff");
            DropColumn("dbo.BusinessTimings", "ClosingTime2_24HoursFormat");
            DropColumn("dbo.BusinessTimings", "ClosingTime2_12HoursFormat");
            DropColumn("dbo.BusinessTimings", "OpeningTime2_24HoursFormat");
            DropColumn("dbo.BusinessTimings", "OpeningTime2_12HoursFormat");
        }
    }
}
