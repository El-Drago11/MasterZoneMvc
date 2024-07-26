namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_67 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.BusinessContentServices1");
            DropTable("dbo.BusinessContentServices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BusinessContentServices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        ServiceTitle = c.String(),
                        ServiceDescription = c.String(),
                        Image = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentServices1",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ServiceTitle = c.String(),
                        ShortDescription = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
