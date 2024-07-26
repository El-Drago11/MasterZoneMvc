namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_121 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessContentMemberShipPackageDetail_PPCMeta", "PlanTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessContentMemberShipPackageDetail_PPCMeta", "PlanTypeId");
        }
    }
}
