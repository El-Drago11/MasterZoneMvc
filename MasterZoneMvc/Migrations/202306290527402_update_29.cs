namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_29 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MainPlans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        PlanDurationTypeKey = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompareAtPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        PlanImage = c.String(),
                        Status = c.Int(nullable: false),
                        PlanPermission = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "OwnerUserLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.StaffAttendances", "BusinessOwnerLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.StaffAttendances", "AttendanceStatus", c => c.Int(nullable: false));
            AddColumn("dbo.StaffAttendances", "AttendanceDate", c => c.String());
            AddColumn("dbo.StaffAttendances", "AttendanceDate_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.StaffAttendances", "AttendanceMonth", c => c.Int(nullable: false));
            AddColumn("dbo.StaffAttendances", "AttendanceYear", c => c.Int(nullable: false));
            AddColumn("dbo.StaffAttendances", "LeaveReason", c => c.String());
            AddColumn("dbo.StaffAttendances", "InTime_24HF", c => c.String());
            AddColumn("dbo.StaffAttendances", "OutTime_24HF", c => c.String());
            AddColumn("dbo.StaffAttendances", "IsApproved", c => c.Int(nullable: false));
            DropColumn("dbo.StaffAttendances", "BusinessOwnerId");
            DropColumn("dbo.StaffAttendances", "Status");
            DropColumn("dbo.StaffAttendances", "Date");
            DropColumn("dbo.StaffAttendances", "Date_DateTimeFormat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StaffAttendances", "Date_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.StaffAttendances", "Date", c => c.String());
            AddColumn("dbo.StaffAttendances", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.StaffAttendances", "BusinessOwnerId", c => c.Long(nullable: false));
            DropColumn("dbo.StaffAttendances", "IsApproved");
            DropColumn("dbo.StaffAttendances", "OutTime_24HF");
            DropColumn("dbo.StaffAttendances", "InTime_24HF");
            DropColumn("dbo.StaffAttendances", "LeaveReason");
            DropColumn("dbo.StaffAttendances", "AttendanceYear");
            DropColumn("dbo.StaffAttendances", "AttendanceMonth");
            DropColumn("dbo.StaffAttendances", "AttendanceDate_DateTimeFormat");
            DropColumn("dbo.StaffAttendances", "AttendanceDate");
            DropColumn("dbo.StaffAttendances", "AttendanceStatus");
            DropColumn("dbo.StaffAttendances", "BusinessOwnerLoginId");
            DropColumn("dbo.Orders", "OwnerUserLoginId");
            DropTable("dbo.MainPlans");
        }
    }
}
