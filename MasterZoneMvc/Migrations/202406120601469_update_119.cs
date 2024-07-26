namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_119 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Licenses", "LicenseDuration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Licenses", "LicenseDuration");
        }
    }
}
