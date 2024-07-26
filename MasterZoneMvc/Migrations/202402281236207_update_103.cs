namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_103 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SportsBookingCheaqueDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        SurName = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        BookedId = c.String(),
                        Department = c.String(),
                        Apartment = c.String(),
                        HouseNumber = c.String(),
                        Message = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SportsBookingCheaqueDetails");
        }
    }
}
