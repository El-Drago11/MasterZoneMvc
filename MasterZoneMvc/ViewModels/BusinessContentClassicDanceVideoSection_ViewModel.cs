using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessContentClassicDanceVideoSection_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Note { get; set; }
        public HttpPostedFile VideoImage { get; set; }
        public string VideoLink { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText1 { get; set; }
        public string ButtonLink1 { get; set; }
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
           else  if (String.IsNullOrEmpty(SubTitle)) { sb.Append(Resources.BusinessPanel.SubTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Note)) { sb.Append(Resources.BusinessPanel.NoteRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(VideoLink)) { sb.Append(Resources.BusinessPanel.VideoLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonText)) { sb.Append(Resources.BusinessPanel.ButtonTextRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonLink)) { sb.Append(Resources.BusinessPanel.ButtonLinkRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonText1)) { sb.Append(Resources.BusinessPanel.ButtonTextRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ButtonLink1)) { sb.Append(Resources.BusinessPanel.ButtonLinkRequired); vm.Valid = false; }
            else if (VideoImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(VideoImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (VideoImage.ContentLength > 10 * 1024 * 1024) // 10 MB
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

    public class BusinessContentClassicDanceVideoDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Note { get; set; }
        public string VideoImage { get; set; }
        public string VideoImageWithPath { get; set; }
        public string VideoLink { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText1 { get; set; }
        public string ButtonLink1 { get; set; }
        public int Mode { get; set; }

    }
}