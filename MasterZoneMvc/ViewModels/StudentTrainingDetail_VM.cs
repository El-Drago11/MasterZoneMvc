using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StudentTrainingDetail_VM
    {
          public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string TrainingImage { get; set; }
        public string TrainingImageWithPath { get; set; }
        public string CertificateIconWithPath { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public int IsCompleted { get; set; }
        public string Duration { get; set; }
        public string TrainingClassDays { get; set; }
        public int TotalLectures { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSeats { get; set; }
        public int TotalCredits { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string Title { get; set; }
        public string LicenseLogoImageWithPath { get; set; }
        public string LicenseCertificateImageWithPath { get; set; }

    }
}