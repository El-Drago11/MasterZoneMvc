using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ClassViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long InstructorLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public int StudentMaxStrength { get; set; }
        public string ClassMode { get; set; }
        public decimal Price { get; set; }
        //public string ScheduledStartOnTime_24HF { get; set; }
        //public string ScheduledEndOnTime_24HF { get; set; }
        //public DateTime ScheduledOnDateTime { get; set; }
        public string ClassDays { get; set; }
        public string OnlineClassLink { get; set; }

        // Location if offline
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }


        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public int IsPaid { get; set; }
        public string ClassPriceType { get; set; }
        public string ClassURLLinkPassword { get; set; }


        //public int ClassDurationSeconds { get; set; }
        public string ClassType { get; set; }
        //public long GroupId { get; set; }
        public string ClassImage { get; set; }
        public string HowToBookText { get; set; }
    }

    public class RequestClassViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long InstructorLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StudentMaxStrength { get; set; }
        public string ClassMode { get; set; }
        public decimal Price { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime ScheduledOnDateTime { get; set; }
        public string ClassDays { get; set; }
        public string OnlineClassLink { get; set; }
        public int IsPaid { get; set; }
        public string ClassPriceType { get; set; }
        public string ClassURLLinkPassword { get; set; }
        public int Mode { get; set; }

        // Location if offline
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string ClassType { get; set; }
        public long GroupId { get; set; }
        public HttpPostedFile ClassImage { get; set; }
        public string HowToBookText { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public List<string> ClassBatches { get; set; }


        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.ValidInformationMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.ErrorMessageClassName); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.ErrorMessageClassDescription); vm.Valid = false; }
            //else if ((StudentMaxStrength <= 0)) { sb.Append(Resources.BusinessPanel.ErrorMessageLimitOfStudent); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ClassMode)) { sb.Append(Resources.BusinessPanel.ErrorMessageOfflineClassLocation); vm.Valid = false; }

            //else if (String.IsNullOrEmpty(ScheduledStartOnTime_24HF)) { sb.Append(Resources.BusinessPanel.ErrorMessageClassStartTime); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ScheduledEndOnTime_24HF)) { sb.Append(Resources.BusinessPanel.ErrorMessageClassEndTime); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ClassDays)) { sb.Append(Resources.BusinessPanel.ErrorMessageSelectionDaysClass); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(OnlineClassLink)) { sb.Append(Resources.BusinessPanel.ErrorMessagOnlineClassURl); vm.Valid = false; }

            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvaildRequestMessage); vm.Valid = false; }
            //else if (DateTime.TryParseExact(ScheduledStartOnTime_24HF, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime) &&
            //   DateTime.TryParseExact(ScheduledEndOnTime_24HF, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTime))
            //{
            //    if (startTime > endTime)
            //    {
            //        sb.Append(Resources.BusinessPanel.ErrorMessageEndTimeGreaterThen);
            //        vm.Valid = false;
            //    }
            //}
            else if (ClassMode == "1")
            {
                if (String.IsNullOrEmpty(OnlineClassLink))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessagOnlineClassURl);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(ClassURLLinkPassword))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageClassOnlineLinkPassword);
                    vm.Valid = false;
                }
            }
            else if (ClassMode == "0")
            {
                if (String.IsNullOrEmpty(Address))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageOfflineClassLocation);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(State))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageClassOfflineState);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(Country))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageClassOfflineCountry);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(City))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageClassOfflineCity);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(Pincode))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageClassOfflinePinCode);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(LandMark))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageClassOfflineLandMark);
                    vm.Valid = false;
                }
            }
            else if (ClassPriceType == "demo")
            {
                Price = 0;
            }
            else if(ClassPriceType != "demo" && Price <= 0)
            {
                sb.Append(Resources.BusinessPanel.ErrorMessageClassPrice); vm.Valid = false;
            }
            else if(ClassCategoryTypeId <= 0)
            {
                sb.Append(Resources.BusinessPanel.ClassCategoryTypeSelection_Required); vm.Valid = false;
            }
            else if(String.IsNullOrEmpty(ClassType))
            {
                sb.Append(Resources.BusinessPanel.ClassTypeSelection_Required); vm.Valid = false;
            }
            else if(String.IsNullOrEmpty(HowToBookText))
            {
                sb.Append(Resources.BusinessPanel.HowToBookText_Required); vm.Valid = false;
            }
            else if (ClassBatches.Count() <= 0)
            {
                sb.Append(Resources.BusinessPanel.ClassBatchSelection_Required); vm.Valid = false;
            }
            else if (ClassImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ClassImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (ClassImage.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidImage;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }

    public class Class_Pagination_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BatchName { get; set; }
        public string InstructorFullName { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public decimal Price { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ClassMode { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string ProfileImage { get; set; }

    }

    public class Class_Pagination_VM_SuperAdminPanel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ClassMode { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string BusinessName { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public int ShowOnHomePage { get; set; }

    }

    public class ClasstList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 UserLoginId { get; set; }
        public long CreatedByLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class Class_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ClassMode { get; set; }
        public string OnlineClassLink { get; set; }
        public decimal Price { get; set; }
        public string ClassDays { get; set; }
        public int StudentMaxStrength { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string LandMark { get; set; }
        public int IsPaid { get; set; }
        public string ClassPriceType { get; set; }
        public string ClassURLLinkPassword { get; set; }
        public long InstructorLoginId { get; set; }
        public int ClassDurationSeconds { get; set; }

        public string ClassType { get; set; }
        public long GroupId { get; set; }
        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string HowToBookText { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public List<long> ClassBatchesIdList { get; set; }
        public List<Batch_VM> ClassBatchList { get; set; }
        public long ParentClassCategoryTypeId { get; set; }
        public int StartDay { get; set; }
        public string StartMonth { get; set; }
        public int StartYear { get; set; }
    }

    public class ClassList_ForHomePage_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ClassMode { get; set; }
        public decimal Price { get; set; }
        public string ClassDays { get; set; }
        public int StudentMaxStrength { get; set; }
        public long InstructorLoginId { get; set; }

        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string ClassDays_ShortForm { get; set; }

        public string BatchName { get; set; }
        public string InstructorFirstName { get; set; }
        public string InstructorLastName { get; set; }
        public string InstructorFullName { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public int InstructorIsCertified { get; set; }
        public string DayAbbr { get; set; }
        public int ShowOnHomePage { get; set; }

    }

    public class ClassSearchList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ClassMode { get; set; }
        public decimal Price { get; set; }
        public string ClassDays { get; set; }
        public int StudentMaxStrength { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string LandMark { get; set; }
        public long InstructorLoginId { get; set; }

        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string ClassDays_ShortForm { get; set; }

        public string BatchName { get; set; }
        public string InstructorFirstName { get; set; }
        public string InstructorLastName { get; set; }
        public string InstructorFullName { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public int InstructorIsCertified { get; set; }
        public string DayAbbr { get; set; }

    }


    public class ClassBookingList_VM
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OnlineClassLink { get; set; }
        public string ClassType { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string BusinessOwnerName { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ClassMode { get; set; }
        public string ClassDays { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string ClassPriceType { get; set; }
        public int TotalRating { get; set; }
        public int TotalReviewsEntered { get; set; }
        public decimal AverageRating { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ClassImage { get; set; }
        public int RecordLimit { get; set; }
        public string ClassImageWithPath { get; set; }
        public long ClassBookingId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Status { get; set; }
        public int? Classpausestatus { get; set; }
        public int CanPauseClass { get; set; }
        public string BusinessName { get; set; }
        public string BusinessProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string GroupName { get; set; }
        public string GroupImageWithPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string MasterId { get; set; }
        public string ImageWithPath { get; set; }
        public string EncryptedUserLoginId { get; set; }
        public string ClassURLLinkPassword { get; set; }


    }
    public class EnrollCourse_VM
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BusinessName { get; set; }
        public string InstructorName { get; set; }
        public string InstructorImageWithPath { get; set; }
        public string InstructorCategoryName { get; set; }
        public int ExpiresInDays { get; set; }

    }

    public class InstructorClassList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string ClassMode { get; set; }
        public string ClassDays { get; set; }
        public string ClassPriceType { get; set; }
    }

    public class ClassList_Filter_Params_BOApp_VM
    {
        public string SearchValue { get; set; }
        public string ClassMode { get; set; }
        public long ClassCategoryTypeId { get; set; }
    }

    public class ClassList_Filter_BOApp_VM
    {
        public long Id { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public string Name { get; set; }
        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
    }

    public class Class_DropdownList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    //// To Get All Classes Detail In Visitor-Panel
    public class BusinessAllClassesDetail
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string FormattedScheduledOnDateTime { get; set; }
        public string Address { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string ClassMode { get; set; }
        public string ClassType { get; set; }
        public decimal Price { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime? ScheduledOnDateTime { get; set; }
        public string ClassDays { get; set; }
        public string ClassImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessOwnerName { get; set; }
        public string StaffName { get; set; }
        public string BusinessProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public int DaysDifference { get; set; }
        public int HoursDifference { get; set; }
        public int MinutesDifference { get; set; }
        public int Rating { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string OnlineClassLink { get; set; }
        public int ScheduledDay { get; set; }
        public string ScheduledMonth { get; set; }
        public string Description { get; set; }
        public int TotalRating { get; set; }
        public decimal AverageRating { get; set; }
        public string Verified { get; set; }
        public int Mode { get; set; }
        public string MenuTag { get; set; }
        public long TotalRecords { get; set; }


    }

    public class ClassDetail_BySearchFilter_VP
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long InstructorLoginId { get; set; }
        public string ClassName { get; set; }
        public string BatchName { get; set; }
        public string ClassCategoryImageWithPath { get; set; }
        public string FormattedScheduledOnDateTime { get; set; }
        public string Address { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string ClassMode { get; set; }
        public string ClassType { get; set; }
        public decimal Price { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime? ScheduledOnDateTime { get; set; }
        public string ClassDays { get; set; }
        public string ClassImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessOwnerName { get; set; }
        public string StaffName { get; set; }
        public string InstructorName { get; set; }
        public string InstructorProfileImage { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public int DaysDifference { get; set; }
        public int HoursDifference { get; set; }
        public int MinutesDifference { get; set; }
        public int Rating { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string OnlineClassLink { get; set; }
        public int ScheduledDay { get; set; }
        public string ScheduledMonth { get; set; }
        public string Description { get; set; }
        public int TotalRating { get; set; }
        public string ParentClassCategoryTypeName { get; set; }
        public decimal AverageRating { get; set; }
        public int Mode { get; set; }
        public string MenuTag { get; set; }
        public long TotalRecords { get; set; }
        public decimal InstructorAverageRating { get; set; }
        public int InstructorIsCertified { get; set; }
        public string ClassCategoryTypeName { get; set; }
        public int ClassDurationSeconds { get; set; }
        public string NextClassDate { get; set; }
        public int NextClassDay { get; set; }
        public string NextClassMonth { get; set; }
        public string BusinessCategoryName { get; set; }




    }

    public class BusinessSingleClassBookingDetail
    {
        public long Id { get; set; }
        public long BatchId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string InstructorName { get; set; }
        public string ClassCategoryImageWithPath { get; set; }
        public string ClassDescription { get; set; }
        public string UniqueUserId { get; set; }

        public string FormattedScheduledOnDateTime { get; set; }
        public string Address { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string ClassMode { get; set; }
        public string ClassType { get; set; }
        public decimal Price { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime ScheduledOnDateTime { get; set; }
        public string ClassDays { get; set; }
        public string ClassImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessOwnerName { get; set; }
        public string BusinessProfileImage { get; set; }
        public int DurationInHours { get; set; }
        public string OnlineClassLink { get; set; }
        public string Description { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public int Verified { get; set; }
        public int IsCertified { get; set; }
        public int MasterPro {  get; set; }

        public string TwitterProfileLink { get; set; }
        public int Mode { get; set; }
        public List<InstructorList_VM> Instructors { get; set; }

        public string About { get; set; }

    }


    public class BusinssClassesTimingDetail
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string BatchName { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ClassDays { get; set; }
        public string BusinessName { get; set; }
        public string InstructorFullName { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }


    public class ClassDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Name { get; set; }
        public string ClassImage { get; set; }
        public decimal Price { get; set; }
        public string ClassImageWithPath { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
    }
    public class ClassdetailsbyInstructor_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string DayName { get; set; }
        public string CurrentDate { get; set; }
        public string ClassDayName { get; set; }
        public string ClassDate { get; set; }
        public string StartDay { get; set; }
        public string NextClassDate { get; set; }
        public string BatchName { get; set; }
        public string Batchstartdate { get; set; }
        public string ClassName { get; set; }
        public decimal ClassDurationHours { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string Address { get; set; }
        public string StartYear { get; set; }
        public string HourDifference { get; set; }
    }
}