namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_62 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApartmentAreas",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ApartmentId = c.Long(nullable: false),
                        Name = c.String(),
                        IsActive = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApartmentBlocks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ApartmentId = c.Long(nullable: false),
                        Name = c.String(),
                        IsActive = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApartmentBlocks");
            DropTable("dbo.ApartmentAreas");
        }
    }
}
