namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_135 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessContentTennis_PPCMeta", "BasicPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BusinessContentTennis_PPCMeta", "CommercialPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BusinessContentTennis_PPCMeta", "OtherPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.BusinessContentTennis_PPCMeta", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BusinessContentTennis_PPCMeta", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.BusinessContentTennis_PPCMeta", "OtherPrice");
            DropColumn("dbo.BusinessContentTennis_PPCMeta", "CommercialPrice");
            DropColumn("dbo.BusinessContentTennis_PPCMeta", "BasicPrice");
        }
    }
}
