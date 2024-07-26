using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class InstructorMasterProfileAboutDetailViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public HttpPostedFile Image { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(SubTitle)) { sb.Append(Resources.BusinessPanel.SubTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonText)) { sb.Append(Resources.BusinessPanel.ButtonTextRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonLink)) { sb.Append(Resources.BusinessPanel.ButtonLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }

            if (Image != null)
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
    public class InstructorMasterProfileAboutDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public int Mode { get; set; }
    }
}