namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationRecords",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NotificationType = c.String(),
                        FromUserLoginId = c.Long(nullable: false),
                        NotificationTitle = c.String(),
                        NotificationText = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NotificationRecordId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        IsRead = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Notifications");
            DropTable("dbo.NotificationRecords");
        }
    }
}
