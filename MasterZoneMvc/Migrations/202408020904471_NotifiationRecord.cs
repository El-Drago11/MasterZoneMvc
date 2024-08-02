namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotifiationRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotificationRecords", "OrderId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotificationRecords", "OrderId");
        }
    }
}
