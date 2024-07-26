namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _update_126 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RoomBookingDetails", "VisitorDetailsId", "dbo.VisitorDetails");
            DropIndex("dbo.RoomBookingDetails", new[] { "VisitorDetailsId" });
            AddColumn("dbo.SportsBookingCheaqueDetails", "TennisTitle", c => c.String());
            AddColumn("dbo.SportsBookingCheaqueDetails", "TennisImage", c => c.String());
            AddColumn("dbo.SportsBookingCheaqueDetails", "RoomTime", c => c.String());
            AddColumn("dbo.SportsBookingCheaqueDetails", "SelectDate", c => c.String());
            AddColumn("dbo.SportsBookingCheaqueDetails", "RoomService", c => c.String());
            DropTable("dbo.RoomBookingDetails");
            DropTable("dbo.VisitorDetails");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.SportsBookingCheaqueDetails", "RoomService");
            DropColumn("dbo.SportsBookingCheaqueDetails", "SelectDate");
            DropColumn("dbo.SportsBookingCheaqueDetails", "RoomTime");
            DropColumn("dbo.SportsBookingCheaqueDetails", "TennisImage");
            DropColumn("dbo.SportsBookingCheaqueDetails", "TennisTitle");
            CreateIndex("dbo.RoomBookingDetails", "VisitorDetailsId");
            AddForeignKey("dbo.RoomBookingDetails", "VisitorDetailsId", "dbo.VisitorDetails", "Id", cascadeDelete: true);
        }
    }
}
