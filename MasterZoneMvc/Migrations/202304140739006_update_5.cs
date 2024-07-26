namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessPlans", "BusinessOwnerLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.BusinessPlans", "PlanImage", c => c.String());
            DropColumn("dbo.BusinessPlans", "BusinessOwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinessPlans", "BusinessOwnerId", c => c.Long(nullable: false));
            DropColumn("dbo.BusinessPlans", "PlanImage");
            DropColumn("dbo.BusinessPlans", "BusinessOwnerLoginId");
        }
    }
}
