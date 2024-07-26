namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_49 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassCategoryTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessCategoryId = c.Long(nullable: false),
                        Name = c.String(),
                        Image = c.String(),
                        IsActive = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Classes", "ClassCategoryTypeId", c => c.Long(nullable: false));
            DropColumn("dbo.Classes", "InstructorLoginId");
            DropColumn("dbo.Classes", "StudentMaxStrength");
            DropColumn("dbo.Classes", "ScheduledStartOnTime_24HF");
            DropColumn("dbo.Classes", "ScheduledEndOnTime_24HF");
            DropColumn("dbo.Classes", "ScheduledOnDateTime");
            DropColumn("dbo.Classes", "ClassDurationSeconds");
            DropColumn("dbo.Classes", "GroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Classes", "GroupId", c => c.Long(nullable: false));
            AddColumn("dbo.Classes", "ClassDurationSeconds", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "ScheduledOnDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Classes", "ScheduledEndOnTime_24HF", c => c.String());
            AddColumn("dbo.Classes", "ScheduledStartOnTime_24HF", c => c.String());
            AddColumn("dbo.Classes", "StudentMaxStrength", c => c.Int(nullable: false));
            AddColumn("dbo.Classes", "InstructorLoginId", c => c.Long(nullable: false));
            DropColumn("dbo.Classes", "ClassCategoryTypeId");
            DropTable("dbo.ClassCategoryTypes");
        }
    }
}
