using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EventDetailViewModel
    {
    }

    public class EventDetails_VM
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string DetailsType { get; set; }
        public HttpPostedFile Image { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Link { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.ValidInformationMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.NameIsRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Link)) { sb.Append(Resources.BusinessPanel.EventDetailLinkRequired_ErrorMessage); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Designation)) { sb.Append(Resources.BusinessPanel.DesignationErrorMessageEventDetails); vm.Valid = false; }


            else if (Image != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Image.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.ErrorMessage.InvaildImageMessage);
                }
                if (Image.ContentLength > 2 * 1024 * 1024) // 10 MB
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

    public class EventDetailsList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string EventDetailsImageWithPath { get; set; }
        public string Link { get; set; }
        public string Designation { get; set; }
        public string DetailsType { get; set; }
    }
}