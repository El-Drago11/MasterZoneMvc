namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_115 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClassBookings", "Repeat_Purchase", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClassBookings", "Repeat_Purchase");
        }
    }
}
