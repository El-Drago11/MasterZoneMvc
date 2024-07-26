namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_99 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Abouts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        AboutTitle = c.String(),
                        AboutDescription = c.String(),
                        OurMissionTitle = c.String(),
                        OurMissionDescription = c.String(),
                        Image = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassReferDetail_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Location1 = c.String(),
                        ContactNumber1 = c.String(),
                        Location2 = c.String(),
                        ContactNumber2 = c.String(),
                        Location3 = c.String(),
                        ContactNumber3 = c.String(),
                        Location4 = c.String(),
                        ContactNumber4 = c.String(),
                        Location5 = c.String(),
                        ContactNumber5 = c.String(),
                        Location6 = c.String(),
                        ContactNumber6 = c.String(),
                        Location7 = c.String(),
                        ContactNumber7 = c.String(),
                        Location8 = c.String(),
                        ContactNumber8 = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        InstructorLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        CourseCategoryId = c.Long(nullable: false),
                        ShortDescription = c.String(),
                        Description = c.String(),
                        CourseMode = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CourseStartDate = c.String(),
                        OnlineCourseLink = c.String(),
                        CoursePriceType = c.String(),
                        HowToBookText = c.String(),
                        GroupId = c.Long(nullable: false),
                        DurationType = c.String(),
                        ExamType = c.String(),
                        ExamId = c.Long(nullable: false),
                        CertificateType = c.String(),
                        CertificateId = c.Long(nullable: false),
                        CertificateProfileId = c.Long(nullable: false),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        LandMark = c.String(),
                        Pincode = c.String(),
                        Latitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Longitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                        IsPaid = c.Int(nullable: false),
                        CourseURLLinkPassword = c.String(),
                        Duration = c.Int(nullable: false),
                        CourseImage = c.String(),
                        IsActive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ContactDetails", "Image", c => c.String());
            AddColumn("dbo.ContactDetails", "Title", c => c.String());
            AddColumn("dbo.ContactDetails", "Description", c => c.String());
            AddColumn("dbo.ContactDetails", "ContactTitle", c => c.String());
            AddColumn("dbo.ContactDetails", "ContactDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContactDetails", "ContactDescription");
            DropColumn("dbo.ContactDetails", "ContactTitle");
            DropColumn("dbo.ContactDetails", "Description");
            DropColumn("dbo.ContactDetails", "Title");
            DropColumn("dbo.ContactDetails", "Image");
            DropTable("dbo.Courses");
            DropTable("dbo.Contacts");
            DropTable("dbo.ClassReferDetail_PPCMeta");
            DropTable("dbo.Abouts");
        }
    }
}
