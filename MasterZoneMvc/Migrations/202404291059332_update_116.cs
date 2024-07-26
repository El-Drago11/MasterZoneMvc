namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_116 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupMessages",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    SenderUserloginId = c.Long(nullable: false),
                    ReceiverUserLoginId = c.Long(nullable: false),
                    Messagebody = c.String(),
                    SenderStatus = c.Int(nullable: false),
                    ReceiverStatus = c.Int(nullable: false),
                    CreatedOn = c.DateTime(nullable: false),
                    CreatedByLoginId = c.Long(nullable: false),
                    UpdatedOn = c.DateTime(nullable: false),
                    UpdatedByLoginId = c.Long(nullable: false),
                    IsDeleted = c.Int(nullable: false),
                    DeletedOn = c.DateTime(nullable: false),
                    GroupId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.Id);
            CreateIndex("dbo.GroupMessages", "GroupId");
            AddForeignKey("dbo.GroupMessages", "GroupId", "dbo.Groups", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropTable("dbo.GroupMessages");
            DropForeignKey("dbo.GroupMessages", "GroupId", "dbo.Groups");
            DropIndex("dbo.GroupMessages", new[] { "GroupId" });
        }
    }
}
