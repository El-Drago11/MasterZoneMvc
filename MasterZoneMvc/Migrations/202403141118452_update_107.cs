namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_107 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TennisAreaTimeSlots", "SlotId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TennisAreaTimeSlots", "SlotId");
        }
    }
}
