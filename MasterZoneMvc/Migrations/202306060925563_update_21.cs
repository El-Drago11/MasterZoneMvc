namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_21 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        ClassId = c.Long(nullable: false),
                        StudentUserLoginId = c.Long(nullable: false),
                        ClassQRCode = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(),
                        Description = c.String(),
                        ClassPriceType = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocumentDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        DocumentTitle = c.String(),
                        DocumentFile = c.String(),
                        RejectionReason = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessOwners", "BusinessSubCategoryId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "LandMark", c => c.String());
            AddColumn("dbo.Classes", "IsPaid", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "ClassPriceType", c => c.String());
            AddColumn("dbo.Classes", "ClassURLLinkPassword", c => c.String());
            DropColumn("dbo.Classes", "FullAddressLocation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Classes", "FullAddressLocation", c => c.String());
            DropColumn("dbo.Classes", "ClassURLLinkPassword");
            DropColumn("dbo.Classes", "ClassPriceType");
            DropColumn("dbo.Classes", "IsPaid");
            DropColumn("dbo.Classes", "LandMark");
            DropColumn("dbo.BusinessOwners", "BusinessSubCategoryId");
            DropTable("dbo.DocumentDetails");
            DropTable("dbo.ClassBookings");
        }
    }
}
