namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessPlanDurationTypes", "Key", c => c.String());
            AddColumn("dbo.BusinessPlans", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessPlans", "Status");
            DropColumn("dbo.BusinessPlanDurationTypes", "Key");
        }
    }
}
