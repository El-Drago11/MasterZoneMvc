namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ParentMenuId = c.Long(nullable: false),
                        Name = c.String(),
                        Image = c.String(),
                        PageLink = c.String(),
                        IsActive = c.Int(nullable: false),
                        Tag = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessCategories", "CategoryKey", c => c.String());
            AddColumn("dbo.BusinessCategories", "ProfilePageTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.BusinessCategories", "MenuTag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessCategories", "MenuTag");
            DropColumn("dbo.BusinessCategories", "ProfilePageTypeId");
            DropColumn("dbo.BusinessCategories", "CategoryKey");
            DropTable("dbo.Menus");
        }
    }
}
