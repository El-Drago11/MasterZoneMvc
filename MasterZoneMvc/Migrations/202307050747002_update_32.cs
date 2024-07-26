namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_32 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Classes", "ClassDurationSeconds", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "ClassType", c => c.String());
            AddColumn("dbo.Classes", "GroupId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "ClassImage", c => c.String());
            AddColumn("dbo.Classes", "HowToBookText", c => c.String());
            AddColumn("dbo.UserLogins", "IsCertified", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "IsCertified");
            DropColumn("dbo.Classes", "HowToBookText");
            DropColumn("dbo.Classes", "ClassImage");
            DropColumn("dbo.Classes", "GroupId");
            DropColumn("dbo.Classes", "ClassType");
            DropColumn("dbo.Classes", "ClassDurationSeconds");
        }
    }
}
