namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_109 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Licenses", "MasterPro", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Licenses", "MasterPro");
        }
    }
}
