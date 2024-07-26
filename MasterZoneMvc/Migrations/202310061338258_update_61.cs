namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_61 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "Gender");
        }
    }
}
