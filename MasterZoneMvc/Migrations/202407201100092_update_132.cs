namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_132 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLogins", "OTP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogins", "OTP");
        }
    }
}
