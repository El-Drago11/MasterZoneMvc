namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_85 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "ResetPasswordToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "ResetPasswordToken");
        }
    }
}
