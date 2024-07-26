namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOwners", "ProfileImage", c => c.String());
            AddColumn("dbo.BusinessOwners", "BusinessLogo", c => c.String());
            AddColumn("dbo.BusinessOwners", "About", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOwners", "About");
            DropColumn("dbo.BusinessOwners", "BusinessLogo");
            DropColumn("dbo.BusinessOwners", "ProfileImage");
        }
    }
}
