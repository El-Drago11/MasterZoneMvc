using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Windows.Input;

namespace MasterZoneMvc.DAL
{
    public class MasterZoneDbContext: DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLoginPermissions>().HasKey(x => new { x.UserLoginId, x.PermissionId });
            modelBuilder.Entity<ClassBatch>().HasKey(x => new { x.BatchId, x.ClassId });
            modelBuilder.Entity<UserLogin>().Property(m => m.Latitude).HasPrecision(18, 15);
            modelBuilder.Entity<UserLogin>().Property(m => m.Longitude).HasPrecision(18, 15);
            modelBuilder.Entity<Class>().Property(m => m.Latitude).HasPrecision(18, 15);
            modelBuilder.Entity<Class>().Property(m => m.Longitude).HasPrecision(18, 15);
            modelBuilder.Entity<Training>().Property(m => m.Longitude).HasPrecision(18, 15);
            modelBuilder.Entity<Training>().Property(m => m.Latitude).HasPrecision(18, 15);
            modelBuilder.Entity<Event>().Property(m => m.Latitude).HasPrecision(18, 15);
            modelBuilder.Entity<Event>().Property(m => m.Longitude).HasPrecision(18, 15);
            modelBuilder.Entity<BusinessCertifications>().HasKey(x => new { x.CertificateId, x.BusinessOwnerLoginId });
        }

        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<BusinessOwner> BusinessOwners { get; set; }
        public DbSet<SubAdmin> SubAdmins { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<BusinessCategory> BusinessCategories { get; set; }
        public DbSet<BusinessDocument> BusinessDocuments { get; set; }
        public DbSet<BusinessDocumentType> BusinessDocumentTypes { get; set; }
        public DbSet<BusinessPlan> BusinessPlans { get; set; }
        public DbSet<BusinessPlanDurationType> BusinessPlanDurationTypes { get; set; }
        public DbSet<BusinessReview> BusinessReviews { get; set; }

        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<StaffCategory> StaffCategories { get; set; }

        public DbSet<ChatThread> ChatThreads { get; set; }
        public DbSet<ThreadMessage> ThreadMessages { get; set; }
        public DbSet<ThreadParticipant> ThreadParticipants { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }

        public DbSet<PaymentMode> PaymentModes { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        public DbSet<CountryMasters> CountryMasters { get; set; }
        public DbSet<StateMasters> StateMasters { get; set; }
        public DbSet<CityMasters> CityMasters { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserLoginPermissions> UserLoginPermissions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationRecord> NotificationRecords { get; set; }
        public DbSet<StudentFavourites> StudentFavourites { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<BusinessStudents> BusinessStudents { get; set; }
        public DbSet<ContactDetails> ContactDetails { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentResponse> PaymentResponses { get; set; }
        public DbSet<PlanBooking> PlanBookings { get; set; }

        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponConsumption> CouponConsumptions { get; set; }

        public DbSet<EventSponsor> ManageEventSponsor { get; set; }
        public DbSet<EventDetail> EventDetails { get; set; }
        public DbSet<EventBooking> EventBooking { get; set; }

        public DbSet<Training> Training { get; set; }
        public DbSet<ClassBooking> ClassBookings { get; set; }
        public DbSet<DocumentDetails> DocumentDetails { get; set; }
        //public DbSet<ProfilePageType> ProfilePageTypes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<LastRecordIdDetail> LastRecordIdDetails { get; set; }
        public DbSet<Query> Queries { get; set; }

        public DbSet<Certificate> Certificate { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<MainPlan> MainPlans { get; set; }
        public DbSet<ClassFeature> ClassFeatures { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<ClassBatch> ClassBatches { get; set; }
        public DbSet<BusinessTiming> BusinessTiming { get; set; }
        public DbSet<BusinessContentVideos> BusinessContentVideos { get; set; }
        public DbSet<BusinessContentImages> BusinessContentImages { get; set; }
        public DbSet<BusinessContentSponsor> BusinessContentSponsors { get; set; }
        //public DbSet<BusinessContentServices> BusinessContentServices { get; set; }

        public DbSet<ExamForm> ExamForms { get; set; }
        public DbSet<ProfilePageType> ProfilePageTypes { get; set; }
        public DbSet<SuperAdminSponsor> SuperAdminSponsors { get; set; }
        public DbSet<UserCertificate> UserCertificates { get; set; }

        public DbSet<Expense> Expense { get; set; }
        public DbSet<EnquiryFollowsUp> EnquiryFollowsUp { get; set; }
        public DbSet<UserContentImages> UserContentImages { get; set; }
        public DbSet<UserContentVideos> UserContentVideos { get; set; }
        public DbSet<MainPlanBooking> MainPlanBooking { get; set; }

        public DbSet<TransferPackage> TransferPackage { get; set; }
        public DbSet<NotificationTransfer> NotificationTransfer { get; set; }
        public DbSet<FollowUser> FollowUser { get; set; }
        public DbSet<ClassCategoryType> ClassCategoryTypes { get; set; }

        public DbSet<BusinessContentVideoCategory> BusinessContentVideoCategories { get; set; }

        public DbSet<License> Licenses { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<UserLoginFeatures> UserLoginFeatures { get; set; }
        public DbSet<ItemFeatures> ItemFeatures { get; set; }
        public DbSet<BusinessCertifications> BusinessCertifications { get; set; }
        public DbSet<FieldTypeCatalog> FieldTypeCatalogs { get; set; }

        public DbSet<LicenseBooking> LicenseBookings { get; set; } 
        public DbSet<Apartment> Apartments { get; set; } 
        public DbSet<ApartmentBooking> ApartmentBookings { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<ApartmentBlock> ApartmentBlocks { get; set; } 
        public DbSet<ApartmentArea> ApartmentAreas { get; set; }
        public DbSet<ExamFormSubmission> ExamFormSubmissions { get; set; }
        public DbSet<InstructorContentDescription_PPCMeta> InstructorContentDescription_PPCMeta { get; set; }

        public DbSet<TrainingBooking> TrainingBooking { get; set; }
        public DbSet<BusinessService> BusinessService { get; set; }
        //public DbSet<ProfilePageData> ProfilePageData { get; set; }
        public DbSet<BusinessContentBanner> BusinessContentBanner { get; set; }
        public DbSet<BusinessContentAbout> BusinessContentAbout { get; set; }
        public DbSet<BusinessContentService> BusinessContentServices { get; set; }
        public DbSet<BusinessContentVideos_PPCMeta> BusinessContentVideos_PPCMeta { get; set; }
        public DbSet<BusinessContentPlan_PPCMeta> BusinessContentPlan_PPCMeta { get; set; }
        public DbSet<BusinessContentAboutServiceDetail> BusinessContentAboutServiceDetail { get; set; }
        public DbSet<BusinessContentTennis_PPCMeta> BusinessContentTennis_PPCMeta { get; set; }
        public DbSet<BusinessContentEvent_PPCMeta> BusinessContentEvent_PPCMeta { get; set; }
        public DbSet<BusinessContentProfessional> BusinessContentProfessional { get; set; }
        public DbSet<BusinessContentReview_PPCMeta> BusinessContentReview_PPCMeta { get; set; }
        public DbSet<BusinessContentClass_PPCMeta> BusinessContentClass_PPCMeta { get; set; }
        public DbSet<BusinessContentWorldClassProgram_PPCMeta> BusinessContentWorldClassProgram_PPCMeta { get; set; }
        public DbSet<BusinessContentWorldClassProgram> BusinessContentWorldClassProgram { get; set; }
        public DbSet<BusinessContentFitnessMovement> BusinessContentFitnessMovement { get; set; }
        public DbSet<BusinessContentFitnessMovement_PPCMeta> BusinessContentLeaderFitness_PPCMeta { get; set; }
        public DbSet<BusinessContentMuchMoreService_PPCMeta> BusinessContentMuchMoreService_PPCMeta { get; set; }
        public DbSet<BusinessContentMuchMoreService> BusinessContentMuchMoreService { get; set; }
        public DbSet<BusinessContentStudioEquipment> BusinessContentStudioEquipment { get; set; }
        public DbSet<BusinessContentStudioEquipment_PPCMeta> BusinessContentStudioEquipment_PPCMeta { get; set; }
        public DbSet<BusinessContentPortfolio> BusinessContentPortfolio { get; set; }
        public DbSet<BusinessContentPortfolio_PPCMeta> BusinessContentPortfolio_PPCMeta { get; set; }
        public DbSet<BusinessContentClient_PPCMeta> BusinessContentClient_PPCMeta { get; set; }
        public DbSet<BussinessContentEventCompanyDetail_PPCMeta> BussinessContentEventCompanyDetail_PPCMeta { get; set; }

        public DbSet<BusinessContentEventImages_PPCMeta> BusinessContentEventImages_PPCMeta { get; set; }
        public DbSet<BusinessContentCourseDetail_PPCMeta> BusinessContentCourseDetail_PPCMeta { get; set; }
        public DbSet<BusinessContentCurriculum_PPCMeta> BusinessContentCurriculum_PPCMeta { get; set; }
        public DbSet<BusinessContentEducation_PPCMeta> BusinessContentEducation_PPCMeta { get; set; }
        public DbSet<BusinessContentLanguage_PPCMeta> BusinessContentLanguage_PPCMeta { get; set; }
        public DbSet<BusinessLanguages> BusinessLanguages { get; set; }
        public DbSet<BusinessUniversity> BusinessUniversity { get; set; }
        public DbSet<BusinessContentUniversity_PPCMeta> BusinessContentUniversity_PPCMeta { get; set; }
        public DbSet<BusinessContentCategories> BusinessContentCategories { get; set; }
        public DbSet<BusinessNoticeBoard> BusinessNoticeBoard { get; set; }
        public DbSet<BusinessCourseCategory> BusinessCourseCategory { get; set; }
        public DbSet<BusinessContentAccessCourseDetail> BusinessContentAccessCourseDetail { get; set; }
        public DbSet<BusinessContentCategoryCource_PPCMeta> BusinessContentCategoryCource_PPCMeta { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<UserFamilyRelation> UserFamilyRelations { get; set; }

        public DbSet<Branch> Branch { get; set; }
        public DbSet<ClassPauseRequest> ClassPauseRequest { get; set; }
        public DbSet<UserExperience> UserExperience { get; set; }
        public DbSet<UserEducation> UserEducation { get; set; }
        public DbSet<UserResumeContent> UserResumeContent { get; set; }
        public DbSet<ClassicDanceTechnique> ClassicDanceTechnique { get; set; }
        public DbSet<BusinessContentClassicDanceTechnique_PPCMeta> BusinessContentClassicDanceTechnique_PPCMeta { get; set; }
        public DbSet<BusinessContentClassicDanceVideoSection_PPCMeta> BusinessContentClassicDanceVideoSection_PPCMeta { get; set; }
        public DbSet<ClassicDanceProfileDetail_PPCMeta> ClassicDanceProfileDetail_PPCMeta { get; set; }
        public DbSet<HomePageFeaturedCardSection> HomePageFeaturedCardSections { get; set; }
        public DbSet<HomePageFeaturedVideo> HomePageFeaturedVideos { get; set; }
        public DbSet<HomePageMultipleItem> HomePageMultipleItems { get; set; }
        public DbSet<HomePageClassCategorySection> HomePageClassCategorySections { get; set; }
        public DbSet<HomePageBannerItem> HomePageBannerItems { get; set; }

        public DbSet<MasterProResume_PPCMeta> MasterProResume_PPCMetas { get; set; }
        public DbSet<MasterProExtraInformation> MasterProExtraInformations { get; set; }
        public DbSet<MasterProContentPdf> MasterProContentPdfs { get; set; }

        public DbSet<BusinessContentExploreDetail_PPCMeta> BusinessContentExploreDetail_PPCMeta { get; set; }
        public DbSet<BusinessContentFindMasterProfileDetail_PPCMeta> BusinessContentFindMasterProfileDetail_PPCMeta { get; set; }
        public DbSet<BusinessContentMasterProfileBanner_PPCMeta> BusinessContentMasterProfileBanner_PPCMeta { get; set; }
        public DbSet<BusinessContentMasterProfileInstructorAboutSection_PPCMeta> BusinessContentMasterProfileInstructorAboutSection_PPCMeta { get; set; }
        public DbSet<BusinessContentTermCondition_PPCMeta> BusinessContentTermCondition_PPCMeta { get; set; }
        public DbSet<BusinessContentMemberShipPackageDetail_PPCMeta> BusinessContentMemberShipPackageDetail_PPCMeta { get; set; }
        public DbSet<BusinessContentMembershipPlan_PPCMeta> BusinessContentMembershipPlan_PPCMeta { get; set; }

        public DbSet<BusinessContentContactInformation_PPCMeta> BusinessContentContactInformation_PPCMeta { get; set; }
        public DbSet<CustomForms> CustomForms { get; set; }
        public DbSet<CustomFormElement> CustomFormElement { get; set; }
        public DbSet<CustomFormOptions> CustomFormOptions { get; set; }
        public DbSet<BusinessCustomForm> BusinessCustomForm { get; set; }
        public DbSet<CustomForm_Record> CustomForm_Record { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }

        public DbSet<ClassReferDetail_PPCMeta> ClassReferDetail_PPCMeta { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<CourseBooking> CourseBookings { get; set; }

        public DbSet<BusinessContentEducationBanner_PPCMeta> BusinessContentEducationBanner_PPCMeta { get; set; }
        public DbSet<SportsBookingCheaqueDetail> SportsBookingCheaqueDetail { get; set; }

        public DbSet<BusinessPackageBooking> BusinessPackageBooking { get; set; }
        public DbSet<TennisAreaTimeSlot> TennisAreaTimeSlot { get; set; }
        public DbSet<GroupMessage> GroupMessage { get; set; }
        public DbSet<ClassIntermediate> ClassIntermediate { get; set; }
        public DbSet<BusinessContentAdvanceMemberShipPackageDetail_PPCMeta> BusinessContentAdvanceMemberShipPackageDetail_PPCMeta { get; set; }
        public DbSet<MasterProfileRoomDetails> MasterProfileRoomDetails { get; set; }
   
    }

}