namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanTypeMainPlain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MainPlans", "PlanType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MainPlans", "PlanType");
        }
    }
}
