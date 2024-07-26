using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MasterProResume_PPCMeta_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Age { get; set; }
        public string Nationality { get; set; }
        public string Freelance { get; set; }
        public string Skype { get; set; }
        public string Languages { get; set; }
        public HttpPostedFile UploadCV { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }



        public MasterProResume_PPCMeta_VM()
        {
            Age = string.Empty;
            Nationality = string.Empty;
            Freelance = string.Empty;
            Skype = string.Empty;
            Languages = string.Empty;


        }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = "Invalid Data!";
            }



            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Age)) { sb.Append(Resources.BusinessPanel.AgeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Nationality)) { sb.Append(Resources.BusinessPanel.NationalityRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Freelance)) { sb.Append(Resources.BusinessPanel.FreelanceRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Skype)) { sb.Append(Resources.BusinessPanel.SkypeRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Languages)) { sb.Append(Resources.BusinessPanel.LanguageRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }



    }
    public class MasterProResume_Pagination_VM
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string BusinessServiceIconWithPath { get; set; }

        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class MasterProResume_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
    public class MasterProResume_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Age { get; set; }
        public string Nationality { get; set; }
        public string Freelance { get; set; }
        public string Skype { get; set; }
        public string Languages { get; set; }
        public string UploadCV { get; set; }
        public string UploadCVImageWithPath { get; set; }
        public int Mode { get; set; }
        public long BusinessOwnerLoginId { get; set; }
    }
}