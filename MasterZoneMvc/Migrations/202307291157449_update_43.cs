namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_43 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SuperAdminSponsors",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SponsorTitle = c.String(),
                        SponsorIcon = c.String(),
                        SponsorLink = c.String(),
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
            DropTable("dbo.SuperAdminSponsors");
        }
    }
}
