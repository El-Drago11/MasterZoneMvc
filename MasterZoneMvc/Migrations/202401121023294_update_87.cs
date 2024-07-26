namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_87 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BusinessOwners", "CoverImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOwners", "CoverImage");
        }
    }
}
