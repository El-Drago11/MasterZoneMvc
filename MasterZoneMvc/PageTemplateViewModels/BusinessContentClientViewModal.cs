using MasterZoneMvc.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentClientViewModal
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public HttpPostedFile ClientImage { get; set; }
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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.NameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
             else if (ClientImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ClientImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (ClientImage.ContentLength > 2 * 1024 * 1024) // 10 MB
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

    public class BusinessContentClientDetail_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientImage { get; set; }
        public string ClientImageWithPath { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentClientDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientImage { get; set; }
        public string ClientImageWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BusinessContentClientDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }


}