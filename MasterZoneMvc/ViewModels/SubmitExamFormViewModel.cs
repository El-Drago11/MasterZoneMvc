using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Automation.Peers;

namespace MasterZoneMvc.ViewModels
{
    public class SubmitExamFormViewModel
    {
    }

    // [C] stand for controller and [A] stand for Action Method and [VM] stand for ViewModel
    public class C_Business_A_SubmitExamForm_VM
    {
        public ExamFormResponse_VM ExamFromDetails { get; set; }
        public ExamFormSubmissionResponse_VM AllDetailsOfCandidate { get; set; }
        public List<ExamFormSubmissionResponse_VM> AllDetailsOfCandidateList { get; set; }
        public List<C_Business_A_SubmitExamForm_ExamTable_VM> SomeDetailsOfCandidateList { get; set; }
    }
    public class C_Business_A_SubmitExamForm_ExamTable_VM
    {
        public long Id { get; set; }
        public string CandidateName { get; set; }
        public string CandidateFather { get; set; }
        public string CandidateMother { get; set; }
        public string CandidateProfileImageWithPath { get; set; }
        public string CurrentRollNo { get; set; }

    }

    public class RequestSubmitExamForm_VM
    {
        public long Id { get; set; }
        public long ExamFormId { get; set; }
        public string SessionYear { get; set; }
        public HttpPostedFile CandidateProfileImage { get; set; }
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
        public HttpPostedFile CandidateSignature { get; set; }
        public HttpPostedFile CandidateGuradianSignature { get; set; }
        public string CandidateGuradianName { get; set; }
        public string BankDraftNo { get; set; }
        public string BankDraftDate { get; set; }
        public string PostalOrderNo { get; set; }
        public HttpPostedFile SuperintendentSignature { get; set; }
        public string SuperintendentName { get; set; }
        public string SuperintendentPinNo { get; set; }
        public string SuperintendentPhoneNo { get; set; }
        public string SuperintendentEmail { get; set; }


        public long BusinessId { get; set; }
        public long BusinessMasterId { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrEmpty(SessionYear)) { sb.Append(Resources.VisitorPanel.ExamForm_SessionYear_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Category)) { sb.Append(Resources.VisitorPanel.ExamForm_Category_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(UserMasterId)) { sb.Append(Resources.VisitorPanel.ExamForm_UserMasterId_RequiredMessage); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(CurrentRollNo)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentRollNo_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CandidateName)) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateName_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CandidateFather)) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateFather_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CandidateMother)) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateMother_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PermanentAddress)) { sb.Append(Resources.VisitorPanel.ExamForm_PermanentAddress_RequiredMessage); vm.Valid = false; }
            else if (PermanentPin <= 0) { sb.Append(Resources.VisitorPanel.ExamForm_PermanentPin_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PermanentMobNo)) { sb.Append(Resources.VisitorPanel.ExamForm_PermanentMobNo_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PresentAddress)) { sb.Append(Resources.VisitorPanel.ExamForm_PresentAddress_RequiredMessage); vm.Valid = false; }
            else if (PresentPin <= 0) { sb.Append(Resources.VisitorPanel.ExamForm_PresentPin_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PresentMobNo)) { sb.Append(Resources.VisitorPanel.ExamForm_PresentMobNo_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Nationality)) { sb.Append(Resources.VisitorPanel.ExamForm_Nationality_RequiredMessage); vm.Valid = false; }
            else if (AadharCardNo <= 0) { sb.Append(Resources.VisitorPanel.ExamForm_AadharCardNo_RequiredMessage); vm.Valid = false; }
            else if (DOB == DateTime.MinValue) { sb.Append(Resources.VisitorPanel.ExamForm_DOB_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.VisitorPanel.ExamForm_Email_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EduQualification)) { sb.Append(Resources.VisitorPanel.ExamForm_EduQualification_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CurrentClass)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentClass_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CurrentSubject)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentSubject_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CurrentCenterName)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentCenterName_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CurrentCenterCity)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentCenterCity_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CurrentCenterCity)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentCenterCity_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PreviousClass)) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousClass_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PreviousSubject)) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousSubject_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PreviousCenterName)) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousCenterName_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PreviousYear.ToString()) || PreviousYear <= 0) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousYear_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PreviousRollNo.ToString()) || PreviousRollNo <= 0) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousRollNo_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PreviousResult.ToString()) || PreviousResult < 0) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousResult_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Amount.ToString()) || Amount <= 0) { sb.Append(Resources.VisitorPanel.ExamForm_PreviousResult_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AmountInWord)) { sb.Append(Resources.VisitorPanel.ExamForm_AmountInWord_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(NoOfAttached.ToString()) || NoOfAttached < 0) { sb.Append(Resources.VisitorPanel.ExamForm_NoOfAttached_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CertificateCollectFrom)) { sb.Append(Resources.VisitorPanel.ExamForm_CertificateCollectFrom_RequiredMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CandidateGuradianName)) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateGuradianName_RequiredMessage); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(BankDraftNo)) { sb.Append(Resources.VisitorPanel.ExamForm_BankDraftNo_RequiredMessage); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(BankDraftDate)) { sb.Append(Resources.VisitorPanel.ExamForm_BankDraftDate_RequiredMessage); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(PostalOrderNo)) { sb.Append(Resources.VisitorPanel.ExamForm_PostalOrderNo_RequiredMessage); vm.Valid = false; }
           
            else if (CandidateProfileImage == null) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateProfileImage_RequiredMessage); vm.Valid = false; }
            else if (CandidateSignature == null) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateSignature_RequiredMessage); vm.Valid = false; }
            else if (CandidateGuradianSignature == null) { sb.Append(Resources.VisitorPanel.ExamForm_CandidateGuradianSignature_RequiredMessage); vm.Valid = false; }
            
            if (vm.Valid && CandidateProfileImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CandidateProfileImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.VisitorPanel.ExamForm_CandidateProfileImage_ValidImageTypesRequired_ErrorMessage);
                }
                else if (CandidateProfileImage.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.VisitorPanel.ExamForm_CandidateProfileImage_FileSizeRequired_ErrorMessage, "2 MB"));
                }

                vm.Valid = isValidImage;
            }

            if (vm.Valid && CandidateSignature != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CandidateSignature.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.VisitorPanel.ExamForm_CandidateSignature_ValidImageTypesRequired_ErrorMessage);
                }
                else if (CandidateSignature.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.VisitorPanel.ExamForm_CandidateSignature_FileSizeRequired_ErrorMessage, "2 MB"));
                }

                vm.Valid = isValidImage;
            }
            if (vm.Valid && CandidateGuradianSignature != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CandidateGuradianSignature.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.VisitorPanel.ExamForm_CandidateGuradianSignature_ValidImageTypesRequired_ErrorMessage);
                }
                else if (CandidateGuradianSignature.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.VisitorPanel.ExamForm_CandidateGuradianSignature_FileSizeRequired_ErrorMessage, "2 MB"));
                }

                vm.Valid = isValidImage;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

        public Error_VM ValidRollNumberInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(CurrentRollNo)) { sb.Append(Resources.VisitorPanel.ExamForm_CurrentRollNo_RequiredMessage); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }


    public class ExamFormSubmissionResponse_VM
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

        public DateTime CreatedOn { get; set; }
        public string CreatedFormatDate { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public string CandidateProfileImageWithPath { get; set; }
        public string CandidateSignatureWithPath { get; set; }
        public string GuradianSignatureWithPath { get; set; }
        public string SuperintendentSignatureWithPath { get; set; }
        public string DOB_FormatDate { get; set; }
    }
}