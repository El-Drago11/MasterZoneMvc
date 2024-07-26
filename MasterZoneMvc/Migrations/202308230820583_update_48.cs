namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_48 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Batches", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Batches", "Status");
        }
    }
}
