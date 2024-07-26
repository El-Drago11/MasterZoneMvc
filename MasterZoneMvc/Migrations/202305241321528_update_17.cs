namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_17 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventId = c.Long(nullable: false),
                        DetailsType = c.String(),
                        Image = c.String(),
                        Name = c.String(),
                        Designation = c.String(),
                        Link = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventSponsors",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventId = c.Long(nullable: false),
                        SponsorTitle = c.String(),
                        SponsorIcon = c.String(),
                        SponsorLink = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Events", "UserLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "Title", c => c.String());
            AddColumn("dbo.Events", "StartDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "EndDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "TotalJoined", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "EventLocationURL", c => c.String());
            AddColumn("dbo.Events", "ShortDescription", c => c.String());
            AddColumn("dbo.Events", "AboutEvent", c => c.String());
            AddColumn("dbo.Events", "AdditionalInformation", c => c.String());
            AddColumn("dbo.Events", "FeaturedImage", c => c.String());
            AddColumn("dbo.Events", "TicketInformation", c => c.String());
            AddColumn("dbo.Events", "Walkings", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "CreatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "UpdatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "IsDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "DeletedOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Events", "Name");
            DropColumn("dbo.Events", "Description");
            DropColumn("dbo.Events", "Venue");
            DropColumn("dbo.Events", "OrganizerName");
            DropColumn("dbo.Events", "InstructorName");
            DropColumn("dbo.Events", "PartnerName");
            DropColumn("dbo.Events", "StartDateTime_DateTimeFormat");
            DropColumn("dbo.Events", "EndDateTime_DateTimeFormat");
            DropColumn("dbo.Events", "CountryId");
            DropColumn("dbo.Events", "StateId");
            DropColumn("dbo.Events", "CityId");
            DropColumn("dbo.Events", "Pincode");
            DropColumn("dbo.Events", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Address", c => c.String());
            AddColumn("dbo.Events", "Pincode", c => c.String());
            AddColumn("dbo.Events", "CityId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "StateId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "CountryId", c => c.Long(nullable: false));
            AddColumn("dbo.Events", "EndDateTime_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "StartDateTime_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "PartnerName", c => c.String());
            AddColumn("dbo.Events", "InstructorName", c => c.String());
            AddColumn("dbo.Events", "OrganizerName", c => c.String());
            AddColumn("dbo.Events", "Venue", c => c.String());
            AddColumn("dbo.Events", "Description", c => c.String());
            AddColumn("dbo.Events", "Name", c => c.String());
            DropColumn("dbo.Events", "DeletedOn");
            DropColumn("dbo.Events", "IsDeleted");
            DropColumn("dbo.Events", "UpdatedByLoginId");
            DropColumn("dbo.Events", "UpdatedOn");
            DropColumn("dbo.Events", "CreatedByLoginId");
            DropColumn("dbo.Events", "CreatedOn");
            DropColumn("dbo.Events", "Walkings");
            DropColumn("dbo.Events", "TicketInformation");
            DropColumn("dbo.Events", "FeaturedImage");
            DropColumn("dbo.Events", "AdditionalInformation");
            DropColumn("dbo.Events", "AboutEvent");
            DropColumn("dbo.Events", "ShortDescription");
            DropColumn("dbo.Events", "EventLocationURL");
            DropColumn("dbo.Events", "TotalJoined");
            DropColumn("dbo.Events", "EndDateTime");
            DropColumn("dbo.Events", "StartDateTime");
            DropColumn("dbo.Events", "Title");
            DropColumn("dbo.Events", "UserLoginId");
            DropTable("dbo.EventSponsors");
            DropTable("dbo.EventDetails");
        }
    }
}
