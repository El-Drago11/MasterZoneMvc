using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EventSponsorViewModel
    {
    }

    public class AddUpdateEventSponsor_Params
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string SponsorTitle { get; set; }
        public HttpPostedFile SponsorIcon { get; set; }
        public string SponsorLink { get; set; }
        public int Mode { get; set; }

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
            if (String.IsNullOrEmpty(SponsorTitle)) { sb.Append("Event Sponsor Title Required!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(SponsorLink)) { sb.Append("Event Sponsor Link Required!"); vm.Valid = false; }

            else if (SponsorIcon != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(SponsorIcon.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (SponsorIcon.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append("Image size too large. Please select a smaller image. (upto 5 MB)");
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
    public class EventSponsor_VM
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string SponsorTitle { get; set; }
        public string SponsorLink { get; set; }
        public string SponsorIcon { get; set; }
        public string EventSponsorWithPath { get; set; }
    }
}