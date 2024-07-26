namespace MasterZoneMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Advertisements",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Image = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsActive = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                        ParentBusinessCategoryId = c.Long(nullable: false),
                        CategoryImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessDocuments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        BusinessDocumentTypeId = c.Long(nullable: false),
                        DocumentPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessDocumentTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Value = c.String(),
                        IsActive = c.Int(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessOwners",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        BusinessName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Address = c.String(),
                        DOB = c.String(),
                        DOB_DateTime = c.DateTime(nullable: false),
                        IsAccountAccepted = c.Int(nullable: false),
                        RejectionReason = c.String(),
                        BusinessCategoryId = c.Long(nullable: false),
                        IsPrimeMember = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessPlanDurationTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessPlans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BusinessPlanDurationTypeId = c.Long(nullable: false),
                        Description = c.String(),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BusinessReviews",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                        Rating = c.Int(nullable: false),
                        Review = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpadatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChatThreads",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ThreadType = c.Int(nullable: false),
                        CreatedByUserLoginId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CityMasters",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        StateID = c.Long(nullable: false),
                        Name = c.String(),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        MaxStrength = c.Int(nullable: false),
                        IsOnlineMode = c.Int(nullable: false),
                        OnlineClassLink = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScheduledOnDate = c.String(),
                        ScheduledOnTime_24HF = c.String(),
                        ScheduledOnDateTime = c.DateTime(nullable: false),
                        CountryId = c.Long(nullable: false),
                        StateId = c.Long(nullable: false),
                        CityId = c.Long(nullable: false),
                        Pincode = c.String(),
                        Address = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CountryMasters",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        CountryCode = c.String(),
                        ISO2 = c.String(),
                        ISO3 = c.String(),
                        NumericCode = c.String(),
                        PhoneCode = c.String(),
                        PhoneCodeWithPlus = c.String(),
                        Capital = c.String(),
                        CurrencyCode = c.String(),
                        CurrencyName = c.String(),
                        CurrencySymbol = c.String(),
                        TLD = c.String(),
                        Native = c.String(),
                        Region = c.String(),
                        Subregion = c.String(),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Enquiries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StudentId = c.Long(nullable: false),
                        BusinessOwnerId = c.Long(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        IsReplied = c.Int(nullable: false),
                        ReplyBody = c.String(),
                        RepliedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Venue = c.String(),
                        IsPaid = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrganizerName = c.String(),
                        InstructorName = c.String(),
                        PartnerName = c.String(),
                        StartDate = c.String(),
                        StartTime_24HF = c.String(),
                        StartDateTime_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndTime_24HF = c.String(),
                        EndDateTime_DateTimeFormat = c.DateTime(nullable: false),
                        CountryId = c.Long(nullable: false),
                        StateId = c.Long(nullable: false),
                        CityId = c.Long(nullable: false),
                        Pincode = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        Name = c.String(),
                        ImagePath = c.String(),
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
                "dbo.PaymentModes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        StaffCategoryId = c.Long(nullable: false),
                        MonthlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfileImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffAttendances",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessOwnerId = c.Long(nullable: false),
                        StaffId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Date = c.String(),
                        Date_DateTimeFormat = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpadatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaffCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StateMasters",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CountryID = c.Long(nullable: false),
                        Name = c.String(),
                        StateCode = c.String(),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        Type = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        IsBlocked = c.Int(nullable: false),
                        ProfileImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentSubscriptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessPlanId = c.Long(nullable: false),
                        StudentId = c.Long(nullable: false),
                        BusinessOwnerId = c.Long(nullable: false),
                        PlanName = c.String(),
                        PlanPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BusinessPlanDurationTypeId = c.Int(nullable: false),
                        BusinessPlanDurationTypeName = c.String(),
                        StartDate = c.String(),
                        StartDate_DateTimeFormat = c.DateTime(nullable: false),
                        EndDate = c.String(),
                        EndDate_DateTimeFormat = c.DateTime(nullable: false),
                        IsPerClassAttended = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpadatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubAdmins",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLoginId = c.Long(nullable: false),
                        Name = c.String(),
                        ProfileImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ThreadMessages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ChatThreadId = c.Long(nullable: false),
                        SenderUserLoginId = c.Long(nullable: false),
                        MessageBody = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ThreadParticipants",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ChatThreadId = c.Long(nullable: false),
                        UserLoginId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumber_CountryCode = c.String(),
                        EmailConfirmed = c.Int(nullable: false),
                        RoleId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByLoginId = c.Long(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedByLoginId = c.Long(nullable: false),
                        IsDeleted = c.Int(nullable: false),
                        DeletedOn = c.DateTime(nullable: false),
                        DeletedByLoginId = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserLogins");
            DropTable("dbo.ThreadParticipants");
            DropTable("dbo.ThreadMessages");
            DropTable("dbo.SubAdmins");
            DropTable("dbo.StudentSubscriptions");
            DropTable("dbo.Students");
            DropTable("dbo.StateMasters");
            DropTable("dbo.StaffCategories");
            DropTable("dbo.StaffAttendances");
            DropTable("dbo.Staffs");
            DropTable("dbo.Roles");
            DropTable("dbo.PaymentModes");
            DropTable("dbo.Groups");
            DropTable("dbo.GroupMembers");
            DropTable("dbo.Events");
            DropTable("dbo.Enquiries");
            DropTable("dbo.CountryMasters");
            DropTable("dbo.Classes");
            DropTable("dbo.CityMasters");
            DropTable("dbo.ChatThreads");
            DropTable("dbo.BusinessReviews");
            DropTable("dbo.BusinessPlans");
            DropTable("dbo.BusinessPlanDurationTypes");
            DropTable("dbo.BusinessOwners");
            DropTable("dbo.BusinessDocumentTypes");
            DropTable("dbo.BusinessDocuments");
            DropTable("dbo.BusinessCategories");
            DropTable("dbo.Advertisements");
        }
    }
}
