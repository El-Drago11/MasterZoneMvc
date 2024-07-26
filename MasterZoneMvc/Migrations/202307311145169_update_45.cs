namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_45 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Classes", "ClassDays_ShortForm", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Classes", "ClassDays_ShortForm");
        }
    }
}
