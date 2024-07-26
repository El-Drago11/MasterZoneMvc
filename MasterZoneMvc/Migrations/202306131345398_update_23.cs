namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LastRecordIdDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Key = c.String(),
                        Prefix = c.String(),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Staffs", "MasterId", c => c.String());
            AddColumn("dbo.UserLogins", "MasterId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "MasterId");
            DropColumn("dbo.Staffs", "MasterId");
            DropTable("dbo.LastRecordIdDetails");
        }
    }
}
