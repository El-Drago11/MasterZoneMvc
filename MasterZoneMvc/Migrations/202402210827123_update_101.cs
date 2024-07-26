namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_101 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        CourseId = c.Long(nullable: false),
                        StudentUserLoginId = c.Long(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(),
                        Description = c.String(),
                        CoursePriceType = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CourseBookings");
        }
    }
}
