namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_118 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "About", c => c.String());
            AddColumn("dbo.UserLogins", "DOB", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "DOB");
            DropColumn("dbo.UserLogins", "About");
        }
    }
}
