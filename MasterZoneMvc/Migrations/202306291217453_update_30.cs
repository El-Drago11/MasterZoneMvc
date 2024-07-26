namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_30 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "GoogleUserId", c => c.String());
            AddColumn("dbo.UserLogins", "FacebookUserId", c => c.String());
            AddColumn("dbo.UserLogins", "GoogleAccessToken", c => c.String());
            AddColumn("dbo.UserLogins", "FacebookAccessToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "FacebookAccessToken");
            DropColumn("dbo.UserLogins", "GoogleAccessToken");
            DropColumn("dbo.UserLogins", "FacebookUserId");
            DropColumn("dbo.UserLogins", "GoogleUserId");
        }
    }
}
