using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MasterProContentVM
    {
    }

    public class RequestMasterProImage_VM
    {
        public long Id { get; set; }
        public string ImageTitle { get; set; }
        public HttpPostedFile Image { get; set; }
        public HttpPostedFile ThumbnailPdf { get; set; }
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
                // Validate Uploaded PDF File
                bool isValidPdf = true;
                string[] validPdfTypes = new string[] { "application/pdf" };
                if (!validPdfTypes.Contains(Image.ContentType))
                {
                    isValidPdf = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Image.ContentLength > 5 * 1024 * 1024) // 5 MB for example, adjust as needed
                {
                    isValidPdf = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "5 MB"));
                }

                vm.Valid = isValidPdf;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class MasterProImageResponse_VM
    {
        public long Id { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public string ThumbnailPdf { get; set; }
        public string ThumbnailPdfWithPath { get; set; }
        public string ImageWithPath { get; set; }

    }
    public class MasterProResponseUserDetail
    {
        public long VideoId { get; set; }
        public long ImageId { get; set; }
        public long CategoryId { get; set; }
        public long RowNum { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string ThumbnailPdf { get; set; }
        public string ThumbnailPdfWithPath { get; set; }
        public string Thumbnail { get; set; }
        public string ImageWithPath { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public long ContentId { get; set; }
        public string ContentType { get; set; }

    }
}