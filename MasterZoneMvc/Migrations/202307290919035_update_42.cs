namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_42 : DbMigration
    {
        public override void Up()
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
                "dbo.BusinessContentSponsors",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        SponsorTitle = c.String(),
                        SponsorIcon = c.String(),
                        SponsorLink = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessOwners", "SpecialDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BusinessOwners", "SpecialPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BusinessOwners", "SpecialDuration", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOwners", "SpecialDuration");
            DropColumn("dbo.BusinessOwners", "SpecialPrice");
            DropColumn("dbo.BusinessOwners", "SpecialDiscount");
            DropTable("dbo.BusinessContentSponsors");
            DropTable("dbo.BusinessContentServices");
        }
    }
}
