namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "GroupImage", c => c.String());
            DropColumn("dbo.Groups", "ImagePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "ImagePath", c => c.String());
            DropColumn("dbo.Groups", "GroupImage");
        }
    }
}
