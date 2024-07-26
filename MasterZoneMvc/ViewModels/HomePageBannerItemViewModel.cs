using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class HomePageBannerItemViewModel
    {
        public long Id { get; set; }
        public string Type { get; set; } // Image, Video
        public string Image { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }

        public string Text { get; set; }
        public string Link { get; set; }
    }

    public class HomePageBannerItem_VM
    {
        public long Id { get; set; }
        public string Type { get; set; } // Image, Video
        public string Image { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public string ImageWithPath { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public class RequestHomePageBannerItem_VM
    {
        public long Id { get; set; }
        public string Type { get; set; } // Image, Video
        public HttpPostedFile Image { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }

        public Error_VM ValidInformation_ImageItem()
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
            if (Mode == 1 && Image == null) { sb.Append(Resources.SuperAdminPanel.ImageRequired); vm.Valid = false; }
            if (Status < 0 || Status > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
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

        public Error_VM ValidInformation_VideoItem()
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
            if (string.IsNullOrEmpty(Video)) { sb.Append(Resources.SuperAdminPanel.VideoLinkRequired); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}