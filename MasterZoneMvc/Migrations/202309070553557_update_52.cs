namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_52 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Staffs", "IsProfessional", c => c.Int(nullable: false));
            AddColumn("dbo.Staffs", "Designation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Staffs", "Designation");
            DropColumn("dbo.Staffs", "IsProfessional");
        }
    }
}
