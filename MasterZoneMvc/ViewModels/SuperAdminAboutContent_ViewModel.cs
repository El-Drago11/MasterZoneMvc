using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SuperAdminAboutContent_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string AboutTitle { get; set; }
        public string AboutDescription { get; set; }
        public string OurMissionTitle { get; set; }
        public string OurMissionDescription { get; set; }
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
            if (String.IsNullOrEmpty(AboutTitle)) { sb.Append(Resources.SuperAdminPanel.AboutTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(AboutDescription)) { sb.Append(Resources.SuperAdminPanel.AboutDescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(OurMissionTitle)) { sb.Append(Resources.SuperAdminPanel.MissionAboutTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(OurMissionDescription)) { sb.Append(Resources.SuperAdminPanel.MissionAboutDescriptionRequired); vm.Valid = false; }
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

    public class SuperAdminAboutDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string AboutTitle { get; set; }
        public string AboutDescription { get; set; }
        public string OurMissionTitle { get; set; }
        public string OurMissionDescription { get; set; }
        public string Image { get; set; }
        public string AboutImageWithPath { get; set; }
        public int Mode { get; set; }
    }
}