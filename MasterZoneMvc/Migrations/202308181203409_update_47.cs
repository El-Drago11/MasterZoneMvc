namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_47 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FollowUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FollowerUserLoginId = c.Long(nullable: false),
                        FollowingUserLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotificationTransfers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransferRequestId = c.Long(nullable: false),
                        TransferSenderId = c.String(),
                        NotificationMessage = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransferPackages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        TransferFromUserloginId = c.Long(nullable: false),
                        TransferToUserLoginId = c.Long(nullable: false),
                        PackageId = c.Long(nullable: false),
                        TransferDate = c.String(),
                        TransferReason = c.String(),
                        RejectionReason = c.String(),
                        TransferStatus = c.Int(nullable: false),
                        TransferType = c.Int(nullable: false),
                        TransferCity = c.String(),
                        TransferState = c.String(),
                        Limit = c.Int(nullable: false),
                        Notification = c.Int(nullable: false),
                        NotificationMessage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransferPackages");
            DropTable("dbo.NotificationTransfers");
            DropTable("dbo.FollowUsers");
        }
    }
}
