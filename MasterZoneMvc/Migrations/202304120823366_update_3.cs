namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ParentPermissionId = c.Long(nullable: false),
                        KeyName = c.String(),
                        TextValue = c.String(),
                        Comments = c.String(),
                        PanelTypeId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserLoginPermissions",
                c => new
                    {
                        UserLoginId = c.Long(nullable: false),
                        PermissionId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserLoginId, t.PermissionId })
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.UserLogins", t => t.UserLoginId, cascadeDelete: true)
                .Index(t => t.UserLoginId)
                .Index(t => t.PermissionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserLoginPermissions", "UserLoginId", "dbo.UserLogins");
            DropForeignKey("dbo.UserLoginPermissions", "PermissionId", "dbo.Permissions");
            DropIndex("dbo.UserLoginPermissions", new[] { "PermissionId" });
            DropIndex("dbo.UserLoginPermissions", new[] { "UserLoginId" });
            DropTable("dbo.UserLoginPermissions");
            DropTable("dbo.Permissions");
        }
    }
}
