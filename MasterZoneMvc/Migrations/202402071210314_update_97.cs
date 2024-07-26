namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_97 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotificationRecords", "ItemId", c => c.Long(nullable: false));
            AddColumn("dbo.NotificationRecords", "ItemTable", c => c.String());
            AddColumn("dbo.NotificationRecords", "IsNotificationLinkable", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotificationRecords", "IsNotificationLinkable");
            DropColumn("dbo.NotificationRecords", "ItemTable");
            DropColumn("dbo.NotificationRecords", "ItemId");
        }
    }
}
