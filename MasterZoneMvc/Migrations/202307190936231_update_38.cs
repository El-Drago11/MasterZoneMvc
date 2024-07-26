namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_38 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Batches", "BusinessOwnerLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Batches", "CreatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Batches", "CreatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Batches", "UpdatedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Batches", "UpdatedByLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Batches", "IsDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.Batches", "DeletedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Batches", "DeletedOn");
            DropColumn("dbo.Batches", "IsDeleted");
            DropColumn("dbo.Batches", "UpdatedByLoginId");
            DropColumn("dbo.Batches", "UpdatedOn");
            DropColumn("dbo.Batches", "CreatedByLoginId");
            DropColumn("dbo.Batches", "CreatedOn");
            DropColumn("dbo.Batches", "BusinessOwnerLoginId");
        }
    }
}
