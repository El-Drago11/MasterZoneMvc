namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessPlans", "CompareAtPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.BusinessPlans", "Discount");
            DropTable("dbo.StudentSubscriptions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StudentSubscriptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessPlanId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                        BusinessOwnerId = c.Long(nullable: false),
                        PlanName = c.String(),
                        PlanPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BusinessPlanDurationTypeId = c.Int(nullable: false),
                        BusinessPlanDurationTypeName = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        IsPerClassAttended = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpadatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessPlans", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.BusinessPlans", "CompareAtPrice");
        }
    }
}
