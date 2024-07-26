using iTextSharp.text.log;
using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateCourse_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long CertificateProfileId { get; set; }
        public long InstructorLoginId { get; set; }
        public long CourseCategoryId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string CourseMode { get; set; }
        public decimal Price { get; set; }
        public string CourseStartDate { get; set; }
        public string OnlineCourseLink { get; set; }
        public string CoursePriceType { get; set; }
        public string HowToBookText { get; set; }
        public long GroupId { get; set; }
        public string DurationType { get; set; }
        public string ExamType { get; set; }
        public long ExamId { get; set; }
        public string CertificateType { get; set; }
        public long CertificateId { get; set; }

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
        public string CourseImage { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public int IsActive { get; set; }
        public  SP_InsertUpdateCourse_Param_VM ()
        {
            Name =  string.Empty;
            BusinessOwnerLoginId = 0;
            InstructorLoginId = 0;
            CourseCategoryId = 0;
            ShortDescription = string.Empty;
            Description = string.Empty;
            CourseMode = string.Empty;
            Price = 0;
            CourseStartDate = string.Empty;
            OnlineCourseLink = string.Empty;
            CoursePriceType = string.Empty;
            HowToBookText = string.Empty;
            GroupId = 0;
            DurationType = string.Empty;
            Duration = 0;
            ExamType = string.Empty;
            ExamId = 0;
            CertificateId = 0;
            CertificateType = string.Empty;
            IsPaid = 0;
            CourseImage = "";
            CourseURLLinkPassword = string.Empty;
            Country = string.Empty;
            State = string.Empty;
            City = string.Empty;
            Address = string.Empty;
            LandMark = string.Empty;
            Pincode = string.Empty;
            Latitude = 0;
            Longitude = 0;
            CertificateProfileId = 0;
        }
    }
}