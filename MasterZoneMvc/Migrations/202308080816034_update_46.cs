namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_46 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EnquiryFollowsUps",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EnquiryId = c.Long(nullable: false),
                        FollowedbyLoginId = c.Long(nullable: false),
                        Comments = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        ExpenseDate = c.String(),
                        ExpenseDate_DateTimeFormat = c.DateTime(nullable: false),
                        Remarks = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MainPlanBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MainPlanId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        PlanDurationTypeKey = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompareAtPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserContentImages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ImageTitle = c.String(),
                        Image = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserContentVideos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        VideoTitle = c.String(),
                        VideoLink = c.String(),
                        VideoThumbnail = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Enquiries", "EnquiryStatus", c => c.Int(nullable: false));
            AddColumn("dbo.SubAdmins", "FirstName", c => c.String());
            AddColumn("dbo.SubAdmins", "LastName", c => c.String());
            AddColumn("dbo.SubAdmins", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.SubAdmins", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.SubAdmins", "CreatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.SubAdmins", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.SubAdmins", "UpdatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.SubAdmins", "IsDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.SubAdmins", "DeletedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserLogins", "LandMark", c => c.String());
            DropColumn("dbo.SubAdmins", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubAdmins", "Name", c => c.String());
            DropColumn("dbo.UserLogins", "LandMark");
            DropColumn("dbo.SubAdmins", "DeletedOn");
            DropColumn("dbo.SubAdmins", "IsDeleted");
            DropColumn("dbo.SubAdmins", "UpdatedByLoginId");
            DropColumn("dbo.SubAdmins", "UpdatedOn");
            DropColumn("dbo.SubAdmins", "CreatedByLoginId");
            DropColumn("dbo.SubAdmins", "CreatedOn");
            DropColumn("dbo.SubAdmins", "Status");
            DropColumn("dbo.SubAdmins", "LastName");
            DropColumn("dbo.SubAdmins", "FirstName");
            DropColumn("dbo.Enquiries", "EnquiryStatus");
            DropTable("dbo.UserContentVideos");
            DropTable("dbo.UserContentImages");
            DropTable("dbo.MainPlanBookings");
            DropTable("dbo.Expenses");
            DropTable("dbo.EnquiryFollowsUps");
        }
    }
}
