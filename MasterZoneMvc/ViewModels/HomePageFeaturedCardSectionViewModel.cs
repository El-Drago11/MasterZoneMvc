using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class HomePageFeaturedCardSectionViewModel
    {
        public long Id { get; set; }
        public string Type { get; set; } // SingleImage, SingleVideo
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
    
    public class HomePageFeaturedCardSection_VM
    {
        public long Id { get; set; }
        public string Type { get; set; } // SingleImage, SingleVideo
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }

        public string ThumbnailImageWithPath { get; set; }

    }

    public class RequestHomePageFeaturedCardSection_VM
    {
        public long Id { get; set; }
        public string Type { get; set; } // SingleImage, SingleVideo
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public HttpPostedFile Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation_SingleImageCard()
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.SuperAdminPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.SuperAdminPanel.DescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonText)) { sb.Append(Resources.SuperAdminPanel.ButtonTextRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonLink)) { sb.Append(Resources.SuperAdminPanel.ButtonLinkRequired); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            else if (Thumbnail != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Thumbnail.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Thumbnail.ContentLength > 10 * 1024 * 1024) // 10 MB
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
        
        public Error_VM ValidInformation_SingleVideoCard()
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.SuperAdminPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.SuperAdminPanel.DescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonText)) { sb.Append(Resources.SuperAdminPanel.ButtonTextRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonLink)) { sb.Append(Resources.SuperAdminPanel.ButtonLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Video)) { sb.Append(Resources.SuperAdminPanel.VideoLinkRequired); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            else if (Thumbnail != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Thumbnail.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Thumbnail.ContentLength > 10 * 1024 * 1024) // 10 MB
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
}