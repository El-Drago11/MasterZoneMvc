namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_117 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessServices", "ServiceType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessServices", "ServiceType");
        }
    }
}
