using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common
{
    public static class StaticResources
    {
        #region File-Save-Paths ----------------------------------------------------------
        public static string FileUploadPath_BusinessCategory = "/Content/Uploads/Images/BusinessCategory/"; 
        public static string FileUploadPath_ClassIntermediate = "/Content/Uploads/Images/ClassIntermediate/"; 
        public static string FileUploadPath_MenuImage = "/Content/Uploads/Images/Menu/"; 
        public static string FileUploadPath_StaffProfileImage = "/Content/Uploads/Images/Staff/";
        public static string FileUploadPath_StudentProfileImage = "/Content/Uploads/Images/StudentProfile/";
        public static string FileUploadPath_BusinessProfileImage = "/Content/Uploads/Images/BusinessProfile/";
        public static string FileUploadPath_BusinessLogo = "/Content/Uploads/Images/BusinessLogo/";
        public static string FileUploadPath_GroupImage = "/Content/Uploads/Images/Group/";
        public static string FileUploadPath_Advertisement = "/Content/Uploads/Images/Advertisement/";
        public static string FileUploadPath_EventImage = "/Content/Uploads/Images/Event/";
        public static string FileUploadPath_EventSponsorIcon = "/Content/Uploads/Images/EventSponsor/";
        public static string FileUploadPath_EventDetailsImage = "/Content/Uploads/Images/EventDetails/";
        public static string FileUploadPath_QRCodeTicketImage = "/Content/Uploads/Images/QRCodeTickets/";
        public static string FileUploadPath_Business = "/Content/Uploads/Documents/";
        public static string FileUploadPath_Certificate = "/Content/Uploads/Certificates/";
        public static string FileUploadPath_MainPlanImage = "/Content/Uploads/Images/MainPlanImage/";
        public static string FileUploadPath_BusinessPlanImage = "/Content/Uploads/Images/BusinessPlans/";
        public static string FileUploadPath_ClassImage = "/Content/Uploads/Images/Class/";
        public static string FileUploadPath_TrainingImage = "/Content/Uploads/Images/Training/";
        public static string FileUploadPath_ClassFeatureIcon = "/Content/Uploads/Images/ClassFeature/";
        public static string FileUploadPath_ManageBusinessImage = "/Content/Uploads/Images/ManageBusinessImages";
        public static string FileUploadPath_VideoThumbNailImage = "/Content/Uploads/Images/VideoThumbNailImage";
        public static string FileUploadPath_ExamFormLogo = "/Content/Uploads/Images/ExamFormLogo/";
        public static string FileUploadPath_SuperAdminSponsorImage = "/Content/Uploads/Images/SuperAdminSponsors/";
        public static string FileUploadPath_BusinessContentServiceImage = "/Content/Uploads/Images/BusinessContentServices/";
        public static string FileUploadPath_SubAdminImage = "/Content/Uploads/Images/SubAdminProfile/";
        public static string FileUploadPath_ClassCategoryTypeImage = "/Content/Uploads/Images/ClassCategoryType/";
        public static string FileUploadPath_LicenseLogo = "/Content/Uploads/Images/LicenseLogo/";
        public static string FileUploadPath_LicenseCertificate = "/Content/Uploads/Images/LicenseCertificate/";
        public static string FileUploadPath_LicenseSignature = "/Content/Uploads/Images/LicenseSignature/";
        public static string FileUploadPath_FamilyMemberProfileImage = "/Content/Uploads/Images/FamilyMemberProfileImage/";
        public static string FileUploadPath_SubmitExamFormProfileImage = "/Content/Uploads/Images/SubmitExamFormProfileImage/";
        public static string FileUploadPath_SubmitExamFormSignature = "/Content/Uploads/Images/SubmitExamFormSignature/";

        public static string FileUploadPath_UserVideoThumbNailImage = "/Content/Uploads/Images/UserVedioThumbNailImage";
        public static string FileUploadPath_UserImageThumbNailImage = "/Content/Uploads/Images/UserImageThumbNailImage";
        public static string FileUploadPath_BusinessServiceImage = "/Content/Uploads/Images/BusinessServiceImage";
        public static string FileUploadPath_BusinessServiceIcon = "/Content/Uploads/Images/BusinessServiceIcon";
        public static string FileUploadPath_BusinessBannerImage = "/Content/Uploads/Images/BusinessBannerImage";
        public static string FileUploadPath_BusinessAboutImage = "/Content/Uploads/Images/BusinessAboutImage";
        public static string FileUploadPath_BusinessAboutServiceIcon = "/Content/Uploads/Images/BusinessAboutServiceIcon";
        public static string FileUploadPath_BusinessTennisImage = "/Content/Uploads/Images/BusinessTennisImage";
        public static string FileUploadPath_BusinessReviewImage = "/Content/Uploads/Images/BusinessReviewImage";
        public static string FileUploadPath_BusinessContentSponsors = "/Content/Uploads/Images/BusinessContentSponsorImage";
        public static string FileUploadPath_BusinessWorldClassProgramImage = "/Content/Uploads/Images/BusinessWorldClassImage";
        public static string FileUploadPath_BusinessFitnessImage = "/Content/Uploads/Images/BusinessFitnessImage";
        public static string FileUploadPath_BusinessMuchMoreServiceIcon = "/Content/Uploads/Images/BusinessMuchMoreServiceIcon";
        public static string FileUploadPath_BusinessPortfolioImage = "/Content/Uploads/Images/BusinessPortfolioImage";
        public static string FileUploadPath_BusinessAudioImage = "/Content/Uploads/Images/BusinessAudioImage";
        public static string FileUploadPath_BusinessAudioFile = "/Content/Uploads/Images/BusinessAudioFile";
        public static string FileUploadPath_BusinessContentClientImage = "/Content/Uploads/Images/BusinessClientImage";
        public static string FileUploadPath_BusinessEventCompanyImage = "/Content/Uploads/Images/BusinessEventCompanyImage";
        public static string FileUploadPath_BusinessEventImage = "/Content/Uploads/Images/BusinessEventImages";
        public static string FileUploadPath_BusinessContentCourseImage = "/Content/Uploads/Images/BusinessCourseImages";
        public static string FileUploadPath_BusinessContentCurriculumImage = "/Content/Uploads/Images/BusinessCurriculumImages";
        public static string FileUploadPath_BusinessContentUniversityLogo = "/Content/Uploads/Images/BusinessUniversityLogo";
        public static string FileUploadPath_BusinessContentLanguageIcon = "/Content/Uploads/Images/BusinessLanguageIcon";
        public static string FileUploadPath_BusinessContentUniversityImage = "/Content/Uploads/Images/BusinessUniversityImage";
        public static string FileUploadPath_BusinessContenTeacherUniversityLogo = "/Content/Uploads/Images/BusinessTeacherUniversityLogo";
        public static string FileUploadPath_BusinessContentCourseCategoryImage = "/Content/Uploads/Images/BusinessCourseCategoryImage";
        public static string FileUploadPath_BusinessAccessImage = "/Content/Uploads/Images/BusinessAccessCourseImage";
        public static string FileUploadPath_SuperAdminProfileImage = "/Content/Uploads/Images/SuperAdminProfile/";
        public static string FileUploadPath_MasterzoneLogoImage = "/Content/Uploads/MasterzoneLogo/";
        public static string FileUploadPath_BusinessTechniqueImage = "/Content/Uploads/Images/ClassicDanceTechniqueImage/";
        public static string FileUploadPath_BusinessClassicDanceVideoImage = "/Content/Uploads/Images/ClassicDanceVideoImage/";
        public static string FileUploadPath_BusinessClassicDanceProfileImage = "/Content/Uploads/Images/ClassicDanceProfileImage/";
        public static string FileUploadPath_BusinessCoverImage = "/Content/Uploads/Images/BusinessCover/";
        public static string FileUploadPath_HomePage = "/Content/Uploads/Images/HomePage/";
        public static string FileUploadPath_HomePageVideos = "/Content/Uploads/Videos/HomePage/";
        public static string FileUploadPath_MasterProPdf = "/Content/Uploads/Images/MasterProPdf/";
        public static string FileUploadPath_MasterProThumbnailPdf = "/Content/Uploads/Images/MasterProThumbnailPdf/";
        public static string FileUploadPath_MasterproUploadCV = "/Content/Uploads/Images/MasterproUploadCV/";
        public static string FileUploadPath_BusinessFindMasterProfileImage = "/Content/Uploads/Images/BusinessExploreMasterProfileImage/";
        public static string FileUploadPath_BusinessInstructorAboutMasterProfileImage = "/Content/Uploads/Images/InstructorMasterProfileAbout/";
        public static string FileUploadPath_BusinessMasterProfileInstructorBannerImage = "/Content/Uploads/Images/BusinessMasterProfileBannerImage/";
        public static string FileUploadPath_BusinessMemberShipPlanImage = "/Content/Uploads/Images/MemberShipPackageImage/";
        public static string FileUploadPath_CourseImage = "/Content/Uploads/Images/BusinessCourseImage";
        public static string FileUploadPath_SuperAdminAboutImage = "/Content/Uploads/Images/SuperAdminAboutImage";
        public static string FileUploadPath_SuperAdminContactImage = "/Content/Uploads/Images/SuperAdminContactImage";
        public static string FileUploadPath_EducationImage = "/Content/Uploads/Images/BusinessEducationProfileImage";


        #endregion

        #region Panel Type Id for Permissions  --------------------------------------------
        public static int PanelType_SuperAdmin = 1;
        public static int PanelType_Business = 2;
        public static int PanelType_User = 3;
        #endregion

        #region Record Limit Constants -----------------------------------------
        public const int RecordLimit_Default = 10;
        public const int RecordLimitTraining_Default = 1;
        #endregion

        #region Category Keys ------------------------------------------------------
        public static string BusinessCategory_CategoryKey_Individual = "individual";
        public static string BusinessCategory_CategoryKey_B2B = "b2b";
        public static string BusinessCategory_CategoryKey_Instructor = "instructor";
        public static string BusinessCategory_CategoryKey_Others = "others";
        #endregion -----------------------------------------------------------------

        #region Max UserId Range ---------------------------------------------------
        public static int UserId_MinValue = 10000000; // 8 digit 
        public static int UserId_MaxValue = 99999999; // 8 digit 
        public static string UserId_Prefix_SuperAdminUser = "1";
        public static string UserId_Prefix_SubAdminUser = "2";
        public static string UserId_Prefix_StudentUser = "3";
        public static string UserId_Prefix_BusinessUser = "4";
        public static string UserId_Prefix_StaffUser = "5";
        #endregion -----------------------------------------------------------------

        #region User Family Relation Type Key Strings ----------------------------
        public static string FRT_Father_KeyName = "FRT_Father";
        public static string FRT_Mother_KeyName = "FRT_Mother";
        public static string FRT_GrandFather_KeyName = "FRT_GrandFather";
        public static string FRT_GrandMother_KeyName = "FRT_GrandMother";
        public static string FRT_Husband_KeyName = "FRT_Husband";
        public static string FRT_Wife_KeyName = "FRT_Wife";
        public static string FRT_Uncle_KeyName = "FRT_Uncle";
        public static string FRT_Aunt_KeyName = "FRT_Aunt";
        public static string FRT_Brother_KeyName = "FRT_Brother";
        public static string FRT_SisterInLaw_KeyName = "FRT_SisterInLaw";
        public static string FRT_Sister_KeyName = "FRT_Sister";
        public static string FRT_BrotherInLaw_KeyName = "FRT_BrotherInLaw";
        public static string FRT_Son_KeyName = "FRT_Son";
        public static string FRT_DaughterInLaw_KeyName = "FRT_DaughterInLaw";
        public static string FRT_Daughter_KeyName = "FRT_Daughter";
        public static string FRT_SonInLaw_KeyName = "FRT_SonInLaw";
        public static string FRT_GrandSon_KeyName = "FRT_GrandSon";
        public static string FRT_Nephew_KeyName = "FRT_Nephew";
        public static string FRT_Niece_KeyName = "FRT_Niece";
        public static string FRT_GrandDaughter_KeyName = "FRT_GrandDaughter";
        public static string FRT_Cousin_KeyName = "FRT_Cousin";
        public static string FRT_FatherInLaw_KeyName = "FRT_FatherInLaw";
        public static string FRT_MotherInLaw_KeyName = "FRT_MotherInLaw";

        #endregion ----------------------------------------------------------
    }
}