namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_82 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        BranchBusinessLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentClassicDanceTechnique_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        TechniqueItemList = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentClassicDanceVideoSection_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        Note = c.String(),
                        VideoImage = c.String(),
                        VideoLink = c.String(),
                        ButtonText = c.String(),
                        ButtonLink = c.String(),
                        ButtonText1 = c.String(),
                        ButtonLink1 = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassicDanceProfileDetail_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        ClassicImage = c.String(),
                        Image = c.String(),
                        ScheduleImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassicDanceTechniques",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        TechniqueImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClassPauseRequests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        ClassBookingId = c.Long(nullable: false),
                        PauseStartDate = c.String(),
                        PauseEndDate = c.String(),
                        PauseDays = c.Int(nullable: false),
                        Reason = c.String(),
                        Status = c.Int(nullable: false),
                        BusinessReply = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserEducations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        SchoolName = c.String(),
                        Designation = c.String(),
                        Description = c.String(),
                        StartMonth = c.String(),
                        StartYear = c.String(),
                        Endmonth = c.String(),
                        EndYear = c.String(),
                        StartDate = c.String(),
                        EndDate = c.String(),
                        Grade = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserExperiences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        CompanyName = c.String(),
                        StartMonth = c.String(),
                        StartYear = c.String(),
                        EndMonth = c.String(),
                        EndYear = c.String(),
                        StartDate = c.String(),
                        EndDate = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserResumeContents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Summary = c.String(),
                        Languages = c.String(),
                        Skills = c.String(),
                        Freelance = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessOwners", "IsBranch", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BusinessOwners", "IsBranch");
            DropTable("dbo.UserResumeContents");
            DropTable("dbo.UserExperiences");
            DropTable("dbo.UserEducations");
            DropTable("dbo.ClassPauseRequests");
            DropTable("dbo.ClassicDanceTechniques");
            DropTable("dbo.ClassicDanceProfileDetail_PPCMeta");
            DropTable("dbo.BusinessContentClassicDanceVideoSection_PPCMeta");
            DropTable("dbo.BusinessContentClassicDanceTechnique_PPCMeta");
            DropTable("dbo.Branches");
        }
    }
}
