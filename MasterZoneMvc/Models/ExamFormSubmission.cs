using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ExamFormSubmission
    {
        [Key]
        public long Id { get; set; }
        public long ExamFormId { get; set; }
        public string SessionYear { get; set; }
        public string CandidateProfileImage { get; set; }
        public string Category { get; set; }
        public string UserMasterId { get; set; }
        public string CurrentRollNo { get; set; }
        public string CandidateName { get; set; }
        public string CandidateFather { get; set; }
        public string CandidateMother { get; set; }
        public string PermanentAddress { get; set; }
        public long PermanentPin { get; set; }
        public string PermanentMobNo { get; set; }
        public string PresentAddress { get; set; }
        public long PresentPin { get; set; }
        public string PresentMobNo { get; set; }
        public string Nationality { get; set; }
        public long AadharCardNo { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string EduQualification { get; set; }
        public string CurrentClass { get; set; }
        public string CurrentSubject { get; set; }
        public string CurrentCenterName { get; set; }
        public string CurrentCenterCity { get; set; }
        public string PreviousClass { get; set; }
        public string PreviousSubject { get; set; }
        public int PreviousYear { get; set; }
        public long PreviousRollNo { get; set; }
        public int PreviousResult { get; set; }
        public string PreviousCenterName { get; set; }
        public int Amount { get; set; }
        public string AmountInWord { get; set; }
        public int NoOfAttached { get; set; }
        public string CertificateCollectFrom { get; set; }
        public string CandidateSignature { get; set; }
        public string CandidateGuradianSignature { get; set; }
        public string CandidateGuradianName { get; set; }
        public string BankDraftNo { get; set; }
        public string BankDraftDate { get; set; }
        public string PostalOrderNo { get; set; }
        public string SuperintendentSignature { get; set; }
        public string SuperintendentName { get; set; }
        public string SuperintendentPinNo { get; set; }
        public string SuperintendentPhoneNo { get; set; }
        public string SuperintendentEmail { get; set; }


        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}