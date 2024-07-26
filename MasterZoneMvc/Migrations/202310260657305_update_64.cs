namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_64 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOwners", "Verified", c => c.Int(nullable: false));
            AddColumn("dbo.UserLogins", "FacebookProfileLink", c => c.String());
            AddColumn("dbo.UserLogins", "TwitterProfileLink", c => c.String());
            AddColumn("dbo.UserLogins", "InstagramProfileLink", c => c.String());
            AddColumn("dbo.UserLogins", "LinkedInProfileLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "LinkedInProfileLink");
            DropColumn("dbo.UserLogins", "InstagramProfileLink");
            DropColumn("dbo.UserLogins", "TwitterProfileLink");
            DropColumn("dbo.UserLogins", "FacebookProfileLink");
            DropColumn("dbo.BusinessOwners", "Verified");
        }
    }
}
