namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_63 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Exams", newName: "ExamForms");
            CreateTable(
                "dbo.ExamFormSubmissions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ExamFormId = c.Long(nullable: false),
                        SessionYear = c.String(),
                        CandidateProfileImage = c.String(),
                        Category = c.String(),
                        UserMasterId = c.String(),
                        CurrentRollNo = c.String(),
                        CandidateName = c.String(),
                        CandidateFather = c.String(),
                        CandidateMother = c.String(),
                        PermanentAddress = c.String(),
                        PermanentPin = c.Long(nullable: false),
                        PermanentMobNo = c.String(),
                        PresentAddress = c.String(),
                        PresentPin = c.Long(nullable: false),
                        PresentMobNo = c.String(),
                        Nationality = c.String(),
                        AadharCardNo = c.Long(nullable: false),
                        DOB = c.DateTime(nullable: false),
                        Email = c.String(),
                        EduQualification = c.String(),
                        CurrentClass = c.String(),
                        CurrentSubject = c.String(),
                        CurrentCenterName = c.String(),
                        CurrentCenterCity = c.String(),
                        PreviousClass = c.String(),
                        PreviousSubject = c.String(),
                        PreviousYear = c.Int(nullable: false),
                        PreviousRollNo = c.Long(nullable: false),
                        PreviousResult = c.Int(nullable: false),
                        PreviousCenterName = c.String(),
                        Amount = c.Int(nullable: false),
                        AmountInWord = c.String(),
                        NoOfAttached = c.Int(nullable: false),
                        CertificateCollectFrom = c.String(),
                        CandidateSignature = c.String(),
                        CandidateGuradianSignature = c.String(),
                        CandidateGuradianName = c.String(),
                        BankDraftNo = c.String(),
                        BankDraftDate = c.String(),
                        PostalOrderNo = c.String(),
                        SuperintendentSignature = c.String(),
                        SuperintendentName = c.String(),
                        SuperintendentPinNo = c.String(),
                        SuperintendentPhoneNo = c.String(),
                        SuperintendentEmail = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ExamForms", "BusinessId", c => c.Long(nullable: false));
            AddColumn("dbo.ExamForms", "CenterNo", c => c.Long(nullable: false));
            AddColumn("dbo.ExamForms", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExamForms", "Status");
            DropColumn("dbo.ExamForms", "CenterNo");
            DropColumn("dbo.ExamForms", "BusinessId");
            DropTable("dbo.ExamFormSubmissions");
            RenameTable(name: "dbo.ExamForms", newName: "Exams");
        }
    }
}
