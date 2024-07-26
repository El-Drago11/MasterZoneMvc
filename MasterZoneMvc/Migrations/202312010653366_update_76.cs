namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_76 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "BusinessStudentProfileImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "BusinessStudentProfileImage");
        }
    }
}
