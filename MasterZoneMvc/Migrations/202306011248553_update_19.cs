namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_19 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        OrderId = c.Long(nullable: false),
                        EventTicketQRCode = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventBookings");
        }
    }
}
