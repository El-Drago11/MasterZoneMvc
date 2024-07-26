namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_108 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessContentTennis_PPCMeta", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessContentTennis_PPCMeta", "Price");
        }
    }
}
