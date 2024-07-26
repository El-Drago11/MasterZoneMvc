namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Staffs", "FirstName", c => c.String());
            AddColumn("dbo.Staffs", "LastName", c => c.String());
            AddColumn("dbo.Staffs", "BusinessOwnerLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Students", "FirstName", c => c.String());
            AddColumn("dbo.Students", "LastName", c => c.String());
            DropColumn("dbo.Staffs", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Staffs", "Name", c => c.String());
            DropColumn("dbo.Students", "LastName");
            DropColumn("dbo.Students", "FirstName");
            DropColumn("dbo.Staffs", "BusinessOwnerLoginId");
            DropColumn("dbo.Staffs", "LastName");
            DropColumn("dbo.Staffs", "FirstName");
        }
    }
}
