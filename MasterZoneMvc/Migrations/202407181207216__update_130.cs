namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_130 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessContentTennis_PPCMeta", "SlotId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessContentTennis_PPCMeta", "SlotId");
        }
    }
}
