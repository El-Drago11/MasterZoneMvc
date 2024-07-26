namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addplanType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessPlans", "planType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessPlans", "planType");
        }
    }
}
