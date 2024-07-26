using Microsoft.Owin;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ExamViewModel
    {
    }
    public class RequestExamFrom_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string EstablishedYear { get; set; }
        public string BusinessLogo { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
        public string SecretaryNumber { get; set; }
        public string RegistrarOfficeNumber { get; set; }
        public string Email { get; set; }
        public string WebsiteLink { get; set; }
        public string ImportantInstruction { get; set; }
        public int Mode { get; set; }
        public HttpPostedFile ExamFormLogo { get; set; }
        public string BusinessMasterId { get; set; }
        public long BusinessId { get; set; }
        public long CenterNo { get; set; }
        public string NameWithAddress { get; set; }
        public int Status { get; set; }

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

            DateTime StartDate_DateTimeFormat = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime EndDate_DateTimeFormat = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.ExamForm_TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EstablishedYear)) { sb.Append(Resources.BusinessPanel.ExamForm_EstablishedYearRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartDate)) { sb.Append(Resources.BusinessPanel.ExamForm_StartDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndDate)) { sb.Append(Resources.BusinessPanel.ExamForm_EndDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(SecretaryNumber)) { sb.Append(Resources.BusinessPanel.ExamForm_SecretaryNumberRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(RegistrarOfficeNumber)) { sb.Append(Resources.BusinessPanel.ExamForm_RegistrarNumberRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(WebsiteLink)) { sb.Append(Resources.BusinessPanel.ExamForm_WebsiteLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.ExamForm_EmailRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ImportantInstruction)) { sb.Append(Resources.BusinessPanel.ExamForm_ImportantInstructionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(BusinessMasterId)) { sb.Append(Resources.BusinessPanel.ExamForm_BusinessMasterIdRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(NameWithAddress)) { sb.Append(Resources.BusinessPanel.ExamForm_NameWithAdddressRequired); vm.Valid = false; }

            else if (StartDate_DateTimeFormat > EndDate_DateTimeFormat) { sb.Append(Resources.BusinessPanel.EndDateDoesNotLessThanStartDateRequired); vm.Valid = false; }
            else if (Mode == 1 && ExamFormLogo == null) { sb.Append(Resources.BusinessPanel.ExamForm_LogoRequired); vm.Valid = false; }
            else if (ExamFormLogo != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ExamFormLogo.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (ExamFormLogo.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
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
    public class ExamFormResponse_VM
    {
        public long Id { get; set; }
        public string BusinessMasterId { get; set; }
        public long BusinessId { get; set; }
        public long CenterNo { get; set; }
        public string Title { get; set; }
        public string EstablishedYear { get; set; }
        public string ExamFormLogo { get; set; }
        public string ExamFormLogoWithPath { get; set; }
        public string StartDate { get; set; }
        public DateTime StartDate_DateTimeFormat { get; set; }
        public string EndDate { get; set; }
        public DateTime EndDate_DateTimeFormat { get; set; }
        public string SecretaryNumber { get; set; }
        public string RegistrarOfficeNumber { get; set; }
        public string Email { get; set; }
        public string WebsiteLink { get; set; }
        public string ImportantInstruction { get; set; }
        public string NameWithAddress { get; set; }
        public int Status { get; set; }

    }
}