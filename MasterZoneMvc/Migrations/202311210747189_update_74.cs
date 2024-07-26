namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_74 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "BlockReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "BlockReason");
        }
    }
}
