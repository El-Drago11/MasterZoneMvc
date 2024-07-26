namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_36 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "UniqueUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "UniqueUserId");
        }
    }
}
