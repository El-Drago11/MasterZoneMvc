namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_25 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enquiries", "UserLoginId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "Name", c => c.String());
            AddColumn("dbo.Enquiries", "Gender", c => c.String());
            AddColumn("dbo.Enquiries", "Email", c => c.String());
            AddColumn("dbo.Enquiries", "DOB", c => c.String());
            AddColumn("dbo.Enquiries", "DOB_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.Enquiries", "PhoneNumber", c => c.String());
            AddColumn("dbo.Enquiries", "AlternatePhoneNumber", c => c.String());
            AddColumn("dbo.Enquiries", "Address", c => c.String());
            AddColumn("dbo.Enquiries", "Activity", c => c.String());
            AddColumn("dbo.Enquiries", "LevelId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "BusinessPlanId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "ClassId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "StartFromDate", c => c.String());
            AddColumn("dbo.Enquiries", "StartFromDate_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.Enquiries", "Status", c => c.String());
            AddColumn("dbo.Enquiries", "StaffId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "FollowUpDate", c => c.String());
            AddColumn("dbo.Enquiries", "FollowUpDate_DateTimeFormat", c => c.DateTime(nullable: false));
            AddColumn("dbo.Enquiries", "Notes", c => c.String());
            DropColumn("dbo.Enquiries", "StudentId");
            DropColumn("dbo.Enquiries", "BusinessOwnerId");
            DropColumn("dbo.Enquiries", "Title");
            DropColumn("dbo.Enquiries", "Description");
            DropColumn("dbo.Enquiries", "IsReplied");
            DropColumn("dbo.Enquiries", "ReplyBody");
            DropColumn("dbo.Enquiries", "RepliedOn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enquiries", "RepliedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.Enquiries", "ReplyBody", c => c.String());
            AddColumn("dbo.Enquiries", "IsReplied", c => c.Int(nullable: false));
            AddColumn("dbo.Enquiries", "Description", c => c.String());
            AddColumn("dbo.Enquiries", "Title", c => c.String());
            AddColumn("dbo.Enquiries", "BusinessOwnerId", c => c.Long(nullable: false));
            AddColumn("dbo.Enquiries", "StudentId", c => c.Long(nullable: false));
            DropColumn("dbo.Enquiries", "Notes");
            DropColumn("dbo.Enquiries", "FollowUpDate_DateTimeFormat");
            DropColumn("dbo.Enquiries", "FollowUpDate");
            DropColumn("dbo.Enquiries", "StaffId");
            DropColumn("dbo.Enquiries", "Status");
            DropColumn("dbo.Enquiries", "StartFromDate_DateTimeFormat");
            DropColumn("dbo.Enquiries", "StartFromDate");
            DropColumn("dbo.Enquiries", "ClassId");
            DropColumn("dbo.Enquiries", "BusinessPlanId");
            DropColumn("dbo.Enquiries", "LevelId");
            DropColumn("dbo.Enquiries", "Activity");
            DropColumn("dbo.Enquiries", "Address");
            DropColumn("dbo.Enquiries", "AlternatePhoneNumber");
            DropColumn("dbo.Enquiries", "PhoneNumber");
            DropColumn("dbo.Enquiries", "DOB_DateTimeFormat");
            DropColumn("dbo.Enquiries", "DOB");
            DropColumn("dbo.Enquiries", "Email");
            DropColumn("dbo.Enquiries", "Gender");
            DropColumn("dbo.Enquiries", "Name");
            DropColumn("dbo.Enquiries", "UserLoginId");
        }
    }
}
