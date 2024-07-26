namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_134 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MasterProfileRoomDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SlotId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        TennisImage = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            DropTable("dbo.MasterProfileRoomDetails");
        }
    }
}
