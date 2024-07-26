namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_89 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HomePageClassCategorySections",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ClassCategoryTypeId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HomePageClassCategorySections");
        }
    }
}
