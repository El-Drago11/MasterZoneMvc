namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_125 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomBookingDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        PlayerCount = c.Int(nullable: false),
                        RoomName = c.String(),
                        BuyDate = c.DateTime(nullable: false),
                        Service = c.String(),
                        VisitorDetailsId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VisitorDetails", t => t.VisitorDetailsId, cascadeDelete: true)
                .Index(t => t.VisitorDetailsId);
            
            CreateTable(
                "dbo.VisitorDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.Long(nullable: false),
                        MasterId = c.String(),
                        Department = c.String(),
                        Address = c.String(),
                        HouseNo = c.String(),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomBookingDetails", "VisitorDetailsId", "dbo.VisitorDetails");
            DropIndex("dbo.RoomBookingDetails", new[] { "VisitorDetailsId" });
            DropTable("dbo.VisitorDetails");
            DropTable("dbo.RoomBookingDetails");
        }
    }
}
