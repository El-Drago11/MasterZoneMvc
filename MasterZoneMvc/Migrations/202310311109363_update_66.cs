namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_66 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BusinessContentAbouts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        Description = c.String(),
                        AboutImage = c.String(),
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
                "dbo.BusinessContentAboutServiceDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        AboutServiceTitle = c.String(),
                        AboutServiceDescription = c.String(),
                        AboutServiceIcon = c.String(),
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
                "dbo.BusinessContentAccessCourseDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        Description = c.String(),
                        CourseImage = c.String(),
                        AccessCourse = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentBanners",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        Description = c.String(),
                        ButtonText = c.String(),
                        ButtonLink = c.String(),
                        BannerImage = c.String(),
                        IsButtonActive = c.String(),
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
                "dbo.BusinessContentCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentCategoryCource_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        CourseCategoryId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentClass_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentClient_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ClientImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentCourseDetail_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        CourseSignIcon = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentCurriculum_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        CurriculumOptions = c.String(),
                        CurriculumImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentEducation_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        University = c.String(),
                        UniversityLogo = c.String(),
                        UniversityImage = c.String(),
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
                "dbo.BusinessContentEvent_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentEventImages_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        EventId = c.Long(nullable: false),
                        Image = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentFitnessMovements",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Requirements = c.String(),
                        Investment = c.String(),
                        Inclusions = c.String(),
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
                "dbo.BusinessContentLanguage_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        LanguageId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentFitnessMovement_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        FitnessImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentMuchMoreServices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentMuchMoreService_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Content = c.String(),
                        ServiceIcon = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentPlan_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        BusinessPlanTitle = c.String(),
                        BusinessPlanDescription = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentPortfolios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        ArtistName = c.String(),
                        AudioFile = c.String(),
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
                "dbo.BusinessContentPortfolio_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        PortfolioImage = c.String(),
                        AudioImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentProfessionals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        ProfessionalTitle = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentReview_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Description = c.String(),
                        ReviewImage = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentServices1",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ServiceTitle = c.String(),
                        ShortDescription = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentStudioEquipments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        EquipmentType = c.String(),
                        EquipmentValue = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentStudioEquipment_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentTennis_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        SubTitle = c.String(),
                        TennisImage = c.String(),
                        Description = c.String(),
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
                "dbo.BusinessContentUniversity_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        UniversityId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentVideos_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        VideoDescription = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentWorldClassPrograms",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        ProfilePageTypeId = c.Long(nullable: false),
                        Title = c.String(),
                        Image = c.String(),
                        Options = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessContentWorldClassProgram_PPCMeta",
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
                "dbo.BusinessCourseCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CourseCategoryImage = c.String(),
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
                "dbo.BusinessLanguages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Language = c.String(),
                        LanguageIcon = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessNoticeBoards",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        StartDate = c.String(),
                        Description = c.String(),
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
                "dbo.BusinessServices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        Icon = c.String(),
                        FeaturedImage = c.String(),
                        Description = c.String(),
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
                "dbo.BusinessUniversities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        UniversityName = c.String(),
                        Qualification = c.String(),
                        StartDate = c.String(),
                        EndDate = c.String(),
                        UniversityLogo = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BussinessContentEventCompanyDetail_PPCMeta",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfilePageTypeId = c.Long(nullable: false),
                        BusinessOwnerLoginId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        EventOptions = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrainingBookings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        TrainingId = c.Long(nullable: false),
                        OrderId = c.Long(nullable: false),
                        TrainingName = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanCompareAtPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsCompleted = c.Int(nullable: false),
                        Duration = c.String(),
                        TrainingClassDays = c.String(),
                        TotalLectures = c.Int(nullable: false),
                        TotalClasses = c.Int(nullable: false),
                        TotalSeats = c.Int(nullable: false),
                        TotalCredits = c.Int(nullable: false),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BusinessContentVideos", "Description", c => c.String());
            AddColumn("dbo.BusinessOwners", "OfficialWebSiteUrl", c => c.String());
            AddColumn("dbo.Trainings", "Duration", c => c.String());
            AddColumn("dbo.Trainings", "TrainingClassDays", c => c.String());
            AddColumn("dbo.Trainings", "TotalLectures", c => c.Int(nullable: false));
            AddColumn("dbo.Trainings", "TotalClasses", c => c.Int(nullable: false));
            AddColumn("dbo.Trainings", "TotalSeats", c => c.Int(nullable: false));
            AddColumn("dbo.Trainings", "TotalCredits", c => c.Int(nullable: false));
            AddColumn("dbo.Trainings", "AdditionalInformation", c => c.String());
            AddColumn("dbo.Trainings", "ExpectationDescription", c => c.String());
            AddColumn("dbo.Trainings", "TrainingRules", c => c.String());
            AddColumn("dbo.Trainings", "BecomeInstructorDescription", c => c.String());
            AddColumn("dbo.UserContentVideos", "VideoDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserContentVideos", "VideoDescription");
            DropColumn("dbo.Trainings", "BecomeInstructorDescription");
            DropColumn("dbo.Trainings", "TrainingRules");
            DropColumn("dbo.Trainings", "ExpectationDescription");
            DropColumn("dbo.Trainings", "AdditionalInformation");
            DropColumn("dbo.Trainings", "TotalCredits");
            DropColumn("dbo.Trainings", "TotalSeats");
            DropColumn("dbo.Trainings", "TotalClasses");
            DropColumn("dbo.Trainings", "TotalLectures");
            DropColumn("dbo.Trainings", "TrainingClassDays");
            DropColumn("dbo.Trainings", "Duration");
            DropColumn("dbo.BusinessOwners", "OfficialWebSiteUrl");
            DropColumn("dbo.BusinessContentVideos", "Description");
            DropTable("dbo.TrainingBookings");
            DropTable("dbo.BussinessContentEventCompanyDetail_PPCMeta");
            DropTable("dbo.BusinessUniversities");
            DropTable("dbo.BusinessServices");
            DropTable("dbo.BusinessNoticeBoards");
            DropTable("dbo.BusinessLanguages");
            DropTable("dbo.BusinessCourseCategories");
            DropTable("dbo.BusinessContentWorldClassProgram_PPCMeta");
            DropTable("dbo.BusinessContentWorldClassPrograms");
            DropTable("dbo.BusinessContentVideos_PPCMeta");
            DropTable("dbo.BusinessContentUniversity_PPCMeta");
            DropTable("dbo.BusinessContentTennis_PPCMeta");
            DropTable("dbo.BusinessContentStudioEquipment_PPCMeta");
            DropTable("dbo.BusinessContentStudioEquipments");
            DropTable("dbo.BusinessContentServices1");
            DropTable("dbo.BusinessContentReview_PPCMeta");
            DropTable("dbo.BusinessContentProfessionals");
            DropTable("dbo.BusinessContentPortfolio_PPCMeta");
            DropTable("dbo.BusinessContentPortfolios");
            DropTable("dbo.BusinessContentPlan_PPCMeta");
            DropTable("dbo.BusinessContentMuchMoreService_PPCMeta");
            DropTable("dbo.BusinessContentMuchMoreServices");
            DropTable("dbo.BusinessContentFitnessMovement_PPCMeta");
            DropTable("dbo.BusinessContentLanguage_PPCMeta");
            DropTable("dbo.BusinessContentFitnessMovements");
            DropTable("dbo.BusinessContentEventImages_PPCMeta");
            DropTable("dbo.BusinessContentEvent_PPCMeta");
            DropTable("dbo.BusinessContentEducation_PPCMeta");
            DropTable("dbo.BusinessContentCurriculum_PPCMeta");
            DropTable("dbo.BusinessContentCourseDetail_PPCMeta");
            DropTable("dbo.BusinessContentClient_PPCMeta");
            DropTable("dbo.BusinessContentClass_PPCMeta");
            DropTable("dbo.BusinessContentCategoryCource_PPCMeta");
            DropTable("dbo.BusinessContentCategories");
            DropTable("dbo.BusinessContentBanners");
            DropTable("dbo.BusinessContentAccessCourseDetails");
            DropTable("dbo.BusinessContentAboutServiceDetails");
            DropTable("dbo.BusinessContentAbouts");
        }
    }
}
