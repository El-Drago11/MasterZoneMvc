namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_98 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "GroupType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "GroupType");
        }
    }
}
