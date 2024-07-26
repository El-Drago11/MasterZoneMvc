using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class RequestUserVedio_VM
    {
        public long Id { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string VideoDescription { get; set; }
        public int Mode { get; set; }
        public HttpPostedFile VideoThumbnail { get; set; }

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
            if (String.IsNullOrEmpty(VideoLink)) { sb.Append(Resources.VisitorPanel.UserVideoLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(VideoTitle)) { sb.Append(Resources.VisitorPanel.UserVideoTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(VideoDescription)) { sb.Append(Resources.VisitorPanel.VedioDescriptionRequired); vm.Valid = false; }
            if (Mode == 1)
            {
             if (VideoThumbnail == null) { sb.Append(Resources.VisitorPanel.UserVideoThumbnailImageRequired); vm.Valid = false; }
            if (VideoThumbnail != null)
                {
                    // Validate Uploaded Image File
                    bool isValidImage = true;
                    string[] validImageTypes = new string[] { "image/jpeg", "image/png" };

                    if (!validImageTypes.Contains(VideoThumbnail.ContentType))
                    {
                        isValidImage = false;
                        sb.Append(Resources.VisitorPanel.ValidImageFileType_ErrorMessage);
                    }
                    else if (VideoThumbnail.ContentLength > 2 * 1024 * 1024) // 2 MB
                    {
                        isValidImage = false;
                        sb.Append(String.Format(Resources.VisitorPanel.ValidFileSize_ErrorMessage, "2 MB"));
                    }

                    vm.Valid = isValidImage;
                }
            }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}