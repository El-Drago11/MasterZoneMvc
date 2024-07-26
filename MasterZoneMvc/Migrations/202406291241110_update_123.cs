namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_123 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessPlans", "PlanTypeTitle", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessPlans", "PlanTypeTitle");
        }
    }
}
