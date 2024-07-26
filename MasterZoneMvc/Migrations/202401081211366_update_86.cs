namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_86 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOwners", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.Certificates", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.MainPlans", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.SuperAdminSponsors", "ShowOnHomePage", c => c.Int(nullable: false));
            AddColumn("dbo.Trainings", "ShowOnHomePage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trainings", "ShowOnHomePage");
            DropColumn("dbo.SuperAdminSponsors", "ShowOnHomePage");
            DropColumn("dbo.MainPlans", "ShowOnHomePage");
            DropColumn("dbo.Events", "ShowOnHomePage");
            DropColumn("dbo.Classes", "ShowOnHomePage");
            DropColumn("dbo.Certificates", "ShowOnHomePage");
            DropColumn("dbo.BusinessOwners", "ShowOnHomePage");
        }
    }
}
