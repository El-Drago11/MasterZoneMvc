namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_60 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApartmentBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        MasterId = c.String(),
                        BatchId = c.Long(nullable: false),
                        ApartmentId = c.Long(nullable: false),
                        BlockName = c.String(),
                        FlatOrVillaNumber = c.String(),
                        Phase = c.String(),
                        Lane = c.String(),
                        OccupantType = c.String(),
                        AreaName = c.String(),
                        Activity = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FamilyMembers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfileImage = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Relation = c.String(),
                        Gender = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FamilyMembers");
            DropTable("dbo.ApartmentBookings");
        }
    }
}
