using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Course
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long InstructorLoginId { get; set; }
        public string Name { get; set; }
        public long CourseCategoryId { get; set; }
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


        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public int IsPaid { get; set; }
        public string CourseURLLinkPassword { get; set; }
        public int Duration { get; set; }
        public string CourseImage { get; set; }
        public int IsActive { get; set; }
    }
}