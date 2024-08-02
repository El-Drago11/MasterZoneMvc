using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using static QRCoder.PayloadGenerator;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessService_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFile FeaturedImage { get; set; }
        public HttpPostedFile Icon { get; set; }
        public int Status { get; set; }  //1 is Enable and 2 is Disable
        public int Mode { get; set; }
        public int ServiceType { get; set; }
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.BusinessServiceTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.BusinessServiceShortDescriptionRequired); vm.Valid = false; }
            if (Mode == 2)
            {

                   if (Status <= 0)
                {

                    sb.Append(Resources.BusinessPanel.BusinessServiceStatusRequired);
                    vm.Valid = false;
                }
            }


            else if (FeaturedImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(FeaturedImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (FeaturedImage.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidImage;
            }
            if (Icon != null)
            {
                // Validate Uploded Image File
                bool isValidLogo = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Icon.ContentType))
                {
                    isValidLogo = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Icon.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidLogo = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidLogo;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class BusinessService_Pagination_VM
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string BusinessServiceIconWithPath { get; set; }
        
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BusinessService_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
    public class BusinessService_ViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FeaturedImage { get;set; }
        public string Icon { get; set; }
        public string BusinessServiceIconWithPath { get; set; }
        public string BusinessServiceImageWithPath { get; set; }
        public string ServiceTitle { get; set; }
        public string ShortDescription { get; set; }
        public int ServiceType { get; set; }
    }
    public class AdminDashBoard_VM
    {
        public long TotalEnrolledStudents { get; set; }
        public long TotalCourseBuyed { get; set; }
        public decimal TotalRevenueGenrated { get; set; }
        public long OnGoingCourse { get; set; }
        public long CurrYearEnrolledStudent { get; set; }

    }

}