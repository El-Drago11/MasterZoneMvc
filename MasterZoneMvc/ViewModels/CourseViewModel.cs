using iTextSharp.text.pdf.qrcode;
using MasterZoneMvc.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CourseViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get;set; }
        public long InstructorLoginId {get;set; }
        public long CourseCategoryId { get;set; }
        public string Name { get;set; }
        public string ShortDescription { get;set; }
        public string Description { get;set; }
        public string CourseMode { get;set; }
        public decimal Price { get;set; }
        public string CoursePriceType { get; set; }
        public string CourseStartDate { get; set; }
        public string OnlineCourseLink { get; set; }
        public string DurationType { get; set; }
        public string HowToBookText { get; set; }
        public string ExamType { get; set; }
        public long ExamId { get; set; }
        public string CertificateType { get; set; }
        public long CertificateId { get; set; }
        public long CertificateProfileId { get; set; }


        // Location if offline
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int IsPaid { get; set; }
        public string CourseURLLinkPassword { get; set; }
        public int Duration { get; set; }
        public HttpPostedFile CourseImage { get; set; }
        public long GroupId { get;set;}
        public int IsActive { get; set; }
        public int Mode { get; set; }
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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.ErrorMessageCourseName); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ShortDescription)) { sb.Append(Resources.BusinessPanel.ErrorMessageShortDescription); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if ((InstructorLoginId <= 0)) { sb.Append(Resources.BusinessPanel.ErrorMessageInstructorLoginId); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CourseMode)) { sb.Append(Resources.BusinessPanel.ErrorMessageOfflineCourseLocation); vm.Valid = false; }
            else if (String.IsNullOrEmpty(DurationType)) { sb.Append(Resources.BusinessPanel.ErrorMessageDurationType); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ExamType)) { sb.Append(Resources.BusinessPanel.ErrorMessageExamType); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CertificateType)) { sb.Append(Resources.BusinessPanel.ErrorMessageCertificateType); vm.Valid = false; }
            else if ((GroupId <= 0)) { sb.Append(Resources.BusinessPanel.ErrorMessageGroupId); vm.Valid = false; }
            else if ((Duration <= 0)) { sb.Append(Resources.BusinessPanel.ErrorMessageDuration); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvaildRequestMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(HowToBookText))
            {
                sb.Append(Resources.BusinessPanel.HowToBookText_CourseRequired); vm.Valid = false;
            }
            else if (CourseMode == "1")
            {
                if (String.IsNullOrEmpty(OnlineCourseLink))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessagOnlineClassURl);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(CourseURLLinkPassword))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageCourseOnlineLinkPassword);
                    vm.Valid = false;
                }
            }
            else if (CourseMode == "0")
            {
                if (String.IsNullOrEmpty(Address))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageOfflineCourseLocation);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(State))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageCourseOfflineState);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(Country))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageCourseOfflineCountry);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(City))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageCourseOfflineCity);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(Pincode))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageCourseOfflinePinCode);
                    vm.Valid = false;
                }
                if (String.IsNullOrEmpty(LandMark))
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageCourseOfflineLandMark);
                    vm.Valid = false;
                }
            }
         
            else if (CourseCategoryId <= 0)
            {
                sb.Append(Resources.BusinessPanel.CourseCategorySelection_Required); vm.Valid = false;
            }
            
            else if (String.IsNullOrEmpty(CourseStartDate))
            {
                sb.Append(Resources.BusinessPanel.CourseStartDate_Required); vm.Valid = false;
            }
            
            else if (CourseImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CourseImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (CourseImage.ContentLength > 10 * 1024 * 1024) // 10 MB
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



        public class CourseDetail_VM
        {
            public long Id { get; set; }
            public long BusinessOwnerLoginId { get; set; }
            public long InstructorLoginId { get; set; }
            public long CourseCategoryId { get; set; }
            public string CourseCatgoryName { get;set; }
            public int Duration { get; set; }

            public string Name { get; set; }
            public string ShortDescription { get; set; }
            public string Description { get; set; }
            public string CourseMode { get; set; }
            public decimal Price { get; set; }
            public string CoursePriceType { get; set; }
            public string CourseStartDate { get; set; }
            public string OnlineCourseLink { get; set; }
            public string DurationType { get; set; }
            public string HowToBookText { get; set; }
            public long? ExamFormId { get; set; }
            public string ExamType { get; set; }
            public long ExamId { get; set; }
            public string CertificateType { get; set; }
            public long CertificateId { get; set; }
            public long CertificateProfileId { get; set; }


            // Location if offline
            public string Country { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public string LandMark { get; set; }
            public string Pincode { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public int IsPaid { get; set; }
            public string CourseURLLinkPassword { get; set; }
            public string CourseImage { get; set; }
            public string CourseImageWithPath { get; set; }
            public long GroupId { get; set; }
            public int IsActive { get; set; }
            public int Mode { get; set; }
        }


        public class CourseDetail_ByPagination
        {
            public long Id { get; set; }
            public long BusinessOwnerLoginId { get; set; }   
            public string Name { get; set; }
            public string ShortDescription { get; set; }
            public string Description { get; set; }
            public string CourseMode { get; set; }
            public decimal Price { get; set; }
            public int Duration { get; set; }
            public string DurationType { get; set; }
            public string CourseImage { get; set; }
            public string CourseImageWithPath { get; set; }
            public int IsActive { get; set; }
            public DateTime CreatedOn { get; set; }
            public string CreatedOn_FormatDate { get; set; }
            public long TotalRecords { get; set; }
            public long SerialNumber { get; set; }
            public int Mode { get; set; }
        }

        public class CourseList_Pagination_SQL_Params_VM
        {
            public Int64 Id { get; set; }
            public Int64 LoginId { get; set; }
            public Int64 BusinessAdminLoginId { get; set; }
            public int Mode { get; set; }
            public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
        }
    }


    public class BusinessCourseDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long InstructorLoginId { get; set; }
        public long CourseCategoryId { get; set; }
        public string Name { get; set; }
        public string CourseCategoryName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string CourseMode { get; set; }
        public decimal Price { get; set; }
        public string CoursePriceType { get; set; }
        public string CourseStartDate { get; set; }
        public string OnlineCourseLink { get; set; }
        public string DurationType { get; set; }
        public string HowToBookText { get; set; }
        public string ExamType { get; set; }
        public long ExamId { get; set; }
        public string CertificateType { get; set; }

        public string ProfileImageWithPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string InstructorName { get; set; }
        public string TwitterProfileLink { get; set; }
        public string CourseURLLinkPassword { get; set; }
        public int Duration { get; set; }
        public string UniqueUserId { get; set; }
        public string Address { get; set; }
        public string CourseImage { get; set; }
        public string CourseImageWithPath { get; set; }
        public long GroupId { get; set; }
        public int IsActive { get; set; }
        public int Mode { get; set; }
    }


    public class BusiessSearchCourse_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Mode { get; set; }
    }


}