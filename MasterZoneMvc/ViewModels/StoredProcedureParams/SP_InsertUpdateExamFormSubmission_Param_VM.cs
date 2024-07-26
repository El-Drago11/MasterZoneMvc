using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateExamFormSubmission_Param_VM
    {
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
        public long BusinessId { get; set; }
        public long BusinessMasterId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateExamFormSubmission_Param_VM()
        {
            SessionYear = string.Empty;
            CandidateProfileImage = string.Empty;
            Category = string.Empty;
            DOB = DateTime.Parse("01/01/1900");
            UserMasterId = string.Empty;
            CurrentRollNo = string.Empty;
            CandidateName = string.Empty;
            CandidateFather = string.Empty;
            CandidateMother = string.Empty;
            PermanentAddress = string.Empty;
            PermanentMobNo = string.Empty;
            PresentAddress = string.Empty;
            PresentMobNo = string.Empty;
            Nationality = string.Empty;
            Email = string.Empty;
            EduQualification = string.Empty;
            CurrentClass = string.Empty;
            CurrentSubject = string.Empty;
            CurrentCenterName = string.Empty;
            CurrentCenterCity = string.Empty;
            PreviousClass = string.Empty;
            AmountInWord = string.Empty;
            PreviousSubject = string.Empty;
            PreviousCenterName = string.Empty;
            CertificateCollectFrom = string.Empty;
            CandidateSignature = string.Empty;
            CandidateGuradianSignature = string.Empty;
            CandidateGuradianName = string.Empty;
            BankDraftNo = string.Empty;
            BankDraftDate = string.Empty;
            PostalOrderNo = string.Empty;
            SuperintendentSignature = string.Empty;
            SuperintendentName = string.Empty;
            SuperintendentPinNo = string.Empty;
            SuperintendentPhoneNo = string.Empty;
            SuperintendentEmail = string.Empty;
        }
    }
}