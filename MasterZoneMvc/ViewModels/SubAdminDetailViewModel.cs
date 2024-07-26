using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SubAdminDetailViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public HttpPostedFile ProfileImage { get; set; }
        public int Mode { get; set; }

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
            if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.SuperAdminPanel.SubAdmin_FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.SuperAdminPanel.SubAdmin_LastNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.SuperAdminPanel.EmailRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.SuperAdminPanel.PasswordRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.SuperAdminPanel.PhoneNumberRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

        public Error_VM VaildInformationProfileImage()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }
            StringBuilder sb = new StringBuilder();
            if (ProfileImage == null)
            {
                vm.Valid = false;
                vm.Message = Resources.SuperAdminPanel.PleaseSelectAnImage_ErrorMessage;
            }
            else if (ProfileImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ProfileImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.SuperAdminPanel.ValidImageFile_ErrorMessage);
                }
                else if (ProfileImage.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.SuperAdminPanel.ValidFileSize_ErrorMessage, "10 MB"));
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
    public class SubAdminProfileDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public int Mode { get; set; }
    }
}