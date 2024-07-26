namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_104 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessPackageBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        PlanId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        PlanName = c.String(),
                        PlanDescription = c.String(),
                        PlanPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanCompareAtPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanDurationTypeId = c.Int(nullable: false),
                        PlanDurationTypeName = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        IsTransfered = c.Int(nullable: false),
                        TransferPackageId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MainPlanBookings", "OrderId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MainPlanBookings", "OrderId");
            DropTable("dbo.BusinessPackageBookings");
        }
    }
}
