using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class AboutServiceViewModel
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string AboutServiceTitle { get; set; }
        public string AboutServiceDescription { get; set; }
        public HttpPostedFile AboutServiceIcon { get; set; }
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
            if (String.IsNullOrEmpty(AboutServiceTitle)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(SubTitle)) { sb.Append(Resources.BusinessPanel.SubTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AboutServiceDescription)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if (AboutServiceIcon != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(AboutServiceIcon.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (AboutServiceIcon.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
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
    public class BusinessContentAboutServiceDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string AboutServiceTitle { get; set; }
        public string AboutServiceDescription { get; set; }
        public string AboutServiceIcon { get; set; }
        public string AboutServiceIconWithPath { get; set; }
    }

    public class AboutServiceDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string AboutServiceTitle { get; set; }
        public string AboutServiceDescription { get; set; }
        public string AboutServiceIcon { get; set; }
        public string AboutServiceIconWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class AboutServiceDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}