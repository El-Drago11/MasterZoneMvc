namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessPlans", "DiscountPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessPlans", "DiscountPercent");
        }
    }
}
