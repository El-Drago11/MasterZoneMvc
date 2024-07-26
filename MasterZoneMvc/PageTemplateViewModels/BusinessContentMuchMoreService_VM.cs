using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentMuchMoreService_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Content { get; set; }
        public HttpPostedFile ServiceIcon { get; set; }
        public int Mode     { get; set; }

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
       
             if (String.IsNullOrEmpty(Content)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
           else if (ServiceIcon != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ServiceIcon.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.ErrorMessage.InvaildImageMessage);
                }
                if (ServiceIcon.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(Resources.ErrorMessage.ImageSizeMessage);
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

    public class BusinessContentMuchMoreServiceDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Content { get; set; }
        public string ServiceIcon { get; set; }
        public string ServiceIconWithPath { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentMuchMoreServiceDetail_Pagination_VM
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string ServiceIcon { get; set; }
        public string ServiceIconWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BusinessContentMuchMoreServiceDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }


    public class BusinessContentMuchMoreServiceDetailViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public int Mode { get; set; }
    }


}