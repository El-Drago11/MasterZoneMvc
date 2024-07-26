namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_81 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOwners", "StudentUserLoginId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOwners", "StudentUserLoginId");
        }
    }
}
