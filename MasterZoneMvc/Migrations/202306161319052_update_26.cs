namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Staffs", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Staffs", "HouseRentAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Staffs", "TravellingAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Staffs", "DearnessAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Staffs", "Remarks", c => c.String());
            DropColumn("dbo.Staffs", "MonthlySalary");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Staffs", "MonthlySalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Staffs", "Remarks");
            DropColumn("dbo.Staffs", "DearnessAllowance");
            DropColumn("dbo.Staffs", "TravellingAllowance");
            DropColumn("dbo.Staffs", "HouseRentAllowance");
            DropColumn("dbo.Staffs", "BasicSalary");
        }
    }
}
