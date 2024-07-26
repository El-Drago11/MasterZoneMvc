namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_65 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InstructorContentDescription_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessOwners", "Experience", c => c.String());
            AddColumn("dbo.BusinessOwners", "Privacy_UniqueUserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOwners", "Privacy_UniqueUserId");
            DropColumn("dbo.BusinessOwners", "Experience");
            DropTable("dbo.InstructorContentDescription_PPCMeta");
        }
    }
}
