namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_95 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CategoryName = c.String(),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventCategories");
        }
    }
}
