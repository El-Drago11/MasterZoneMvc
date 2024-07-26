using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessContentViewModel
    {
    }

    public class RequestBusinessVideo_VM
    {
        public long Id { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public int Mode { get; set; }
        public HttpPostedFile VideoThumbnail { get; set; }
        public long VideoCategoryId { get; set; }
        public string Description { get; set; }


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
            if (String.IsNullOrEmpty(VideoLink)) { sb.Append(Resources.BusinessPanel.BusinessVideoLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(VideoTitle)) { sb.Append(Resources.BusinessPanel.BusinessVideoTitleRequired); vm.Valid = false; }
            else if (VideoCategoryId <= 0) { sb.Append(Resources.BusinessPanel.BusinessVideoCategoryRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if (VideoThumbnail == null) { sb.Append(Resources.BusinessPanel.BusinessVideoThumbnailImageRequired); vm.Valid = false; }
            else if (VideoThumbnail != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(VideoThumbnail.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (VideoThumbnail.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
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

    public class BusinessVideoResponse_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string VideoThumbnail { get; set; }
        public string VideoThumbNailImageWithPath { get; set; }
        public long VideoCategoryId { get; set; }
        public string VideoCategoryName { get; set; }
        public string Description { get; set; }

    }

    public class BusinessVideoByCategory_VM
    {
        public long VideoCategoryId { get; set; }
        public string VideoCategoryName { get; set; }
        public List<BusinessVideoResponse_VM> Videos { get; set; }

    }

    public class RequestBusinessImage_VM
    {
        public long Id { get; set; }
        public string ImageTitle { get; set; }
        public HttpPostedFile Image { get; set; }
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
            if (String.IsNullOrEmpty(ImageTitle)) { sb.Append(Resources.BusinessPanel.BusinessImageTitleRequired); vm.Valid = false; }
            else if (Image == null) { sb.Append(Resources.BusinessPanel.BusinessImageRequired); vm.Valid = false; }
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
                else if (Image.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
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

    public class BusinessImageResponse_VM
    {
        public long Id { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }

    }

    public class BusinessResponseUserDetail
    {
        public long VideoId { get; set; }
        public long ImageId { get; set; }
        public long CategoryId { get; set; }
        public long RowNum { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Thumbnail { get; set; }
        public string Image { get; set; }
        public string VideoThumbNailImageWithPath { get; set; }
        public string ImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

    }

}