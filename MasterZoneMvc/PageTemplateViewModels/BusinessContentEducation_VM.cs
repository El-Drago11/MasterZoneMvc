using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentEducation_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string University { get; set; }
        public HttpPostedFile UniversityLogo { get; set; }
        public HttpPostedFile UniversityImage { get; set; }
        public string Description { get; set; }
       
        public int Mode { get; set; }
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
           if (String.IsNullOrEmpty(University)) { sb.Append(Resources.BusinessPanel.UniversityRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(StartDate)) { sb.Append(Resources.BusinessPanel.StartDateRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(EndDate)) { sb.Append(Resources.BusinessPanel.EndDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if (UniversityLogo != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(UniversityLogo.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (UniversityLogo.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append("Image size too large. Please select a smaller image. (upto 5 MB)");
                }

                vm.Valid = isValidImage;
            }

            else if (UniversityImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(UniversityImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (UniversityImage.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append("Image size too large. Please select a smaller image. (upto 5 MB)");
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

    public class BusinessContentEducationDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string University { get; set; }
        public string UniversityLogo { get; set; }
        public string UniversityLogoWithPath { get; set; }
        public string UniversityImage { get; set; }
        public string UniversityImageWithPath { get; set; }
        public string Description { get; set; }
        public long UniversityId { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentEducationDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string University { get; set; }
      
        public string UniversityLogo { get; set; }
        public string UniversityLogoWithPath { get; set; }
        public string UniversityImage { get; set; }
        public string UniversityImageWithPath { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }
    public class BusinessContentEducationDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}