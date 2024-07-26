namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_79 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassCategoryTypes", "ParentClassCategoryTypeId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClassCategoryTypes", "ParentClassCategoryTypeId");
        }
    }
}
