namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_120 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassIntermediates",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        Description = c.String(),
                        Image = c.String(),
                        ImageTitle = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ClassIntermediates");
        }
    }
}
