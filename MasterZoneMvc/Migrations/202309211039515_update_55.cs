namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_55 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FieldTypeCatalogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ParentId = c.Long(nullable: false),
                        PanelTypeId = c.Long(nullable: false),
                        KeyName = c.String(),
                        TextValue = c.String(),
                        IsActive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FieldTypeCatalogs");
        }
    }
}
