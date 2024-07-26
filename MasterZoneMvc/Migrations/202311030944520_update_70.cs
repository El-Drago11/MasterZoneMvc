namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_70 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFamilyRelations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        User1LoginId = c.Long(nullable: false),
                        User2LoginId = c.Long(nullable: false),
                        User1Relation_FieldTypeCatalogKey = c.String(),
                        User2Relation_FieldTypeCatalogKey = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserFamilyRelations");
        }
    }
}
