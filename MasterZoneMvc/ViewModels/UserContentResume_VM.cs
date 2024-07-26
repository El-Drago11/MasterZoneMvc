using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserContentResume_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Summary { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public int Freelance { get; set; }
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
            if (String.IsNullOrEmpty(Summary)) { sb.Append(Resources.BusinessPanel.SummaryRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Languages)) { sb.Append(Resources.BusinessPanel.LanguageRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Skills)) { sb.Append(Resources.BusinessPanel.SkillsRequired); vm.Valid = false; }
            else  if (Freelance <= 0) { sb.Append(Resources.BusinessPanel.FreelanceRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }


    public class BusinessContentUserResumeDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Summary { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public int Freelance { get; set; }
        public int Mode { get; set; }
    }

    public class StaffUserContentUserResumeDetail_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Summary { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public int Freelance { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set;}
        public string FacebookProfileList { get; set; }
        public string InstagramProfileList { get; set; }
        public string LinkedInProfileList { get; set; }
        public string TwitterProfileList { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StaffCategoryName { get; set; }
        public string MasterId { get; set; }
        public string Country { get; set; }
        public string Email {get;set;}
        public string PhoneNumber {get;set;}
        public string DOB { get;set;}
        public int Mode { get; set; }
    }
}