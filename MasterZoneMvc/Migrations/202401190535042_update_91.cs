namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_91 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MasterProContentPdfs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        ImageTitle = c.String(),
                        Image = c.String(),
                        ThumbnailPdf = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MasterProExtraInformations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        ShortDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MasterProResume_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Age = c.String(),
                        Nationality = c.String(),
                        UploadCV = c.String(),
                        Freelance = c.String(),
                        Skype = c.String(),
                        Languages = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MasterProResume_PPCMeta");
            DropTable("dbo.MasterProExtraInformations");
            DropTable("dbo.MasterProContentPdfs");
        }
    }
}
