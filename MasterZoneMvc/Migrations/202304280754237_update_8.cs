namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        PaymentModeType = c.String(),
                        PaymentModeDetail = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentFavourites",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StudentLoginId = c.Long(nullable: false),
                        FavouriteUserLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Enquiries", "CreatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Enquiries", "UpdatedByLoginId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enquiries", "UpdatedByLoginId");
            DropColumn("dbo.Enquiries", "UpdatedOn");
            DropColumn("dbo.Enquiries", "CreatedByLoginId");
            DropTable("dbo.StudentFavourites");
            DropTable("dbo.PaymentDetails");
        }
    }
}
