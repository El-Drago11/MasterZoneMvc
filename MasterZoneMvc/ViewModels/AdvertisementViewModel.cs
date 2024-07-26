using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class AdvertisementViewModel
    {
    }

    public class AdvertisementProfile_VM
    {
        public long Id { get; set; }
        public HttpPostedFile Image { get; set; }
        public string AdvertisementLink { get; set; }
        public string ImageOrientationType { get; set; }
        public string AdvertisementCategory { get; set; }
        public string AdvertisementCategoryName { get; set; }

        public int IsActive { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }

            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(ImageOrientationType)) { sb.Append(Resources.BusinessPanel.ImageOrientationTypeNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AdvertisementCategory)) { sb.Append(Resources.BusinessPanel.AdvertisementCategoryRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AdvertisementCategoryName)) { sb.Append(Resources.BusinessPanel.AdvertisementCategoryNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AdvertisementLink)) { sb.Append(Resources.BusinessPanel.AdvertisementLinkRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(AdvertisementLink)) { sb.Append(Resources.SuperAdminPanel.ValidLinkRequired); vm.Valid = false; }
            else if (IsActive < 0 || IsActive > 1) { sb.Append(Resources.BusinessPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            else if (Image != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Image.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Image.ContentLength > 10 * 1024 * 1024) // 10 MB
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
    public class Advertisement_VM
    {
        public Int64 Id { get; set; }
        public string AdvertisementLink { get; set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }
        public string ImageOrientationType { get; set; }
        public int IsActive { get; set; }
        public string AdvertisementCategory { get; set; }
        public string AdvertisementCategoryName { get; set; }


    }

    public class Advertisement_Pagination_VM
    {
        public Int64 Id { get; set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }
        public string ImageOrientationType { get; set; }
        public int Status { get; set; }
        public string AdvertisementCategory { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public long CreatedForLoginId { get; set; }

        public string AdvertisementCategoryName { get; set; }
        public string AdvertisementLink { get; set; }


    }

    public class AdvertisementList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 CreatedForLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class SingleUserAdvertisement_VM
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string AdvertisementImageWithPath { get; set; }
        public string ImageOrientationType { get; set; }
        public string AdvertisementCategory { get; set; }
        public string AdvertisementCategoryName { get; set; }
        public string AdvertisementLink { get; set; }

    }
}