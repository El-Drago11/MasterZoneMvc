using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ContactDetailViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string ContactImageWithPath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContactTitle { get; set; }
        public string ContactDescription { get; set; }
    }

    public class ContactUsMessage_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public long businessOwnerLoginId { get; set; }

        public Error_VM ValidInformation()
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

            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.VisitorPanel.NameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EmailAddress)) { sb.Append(Resources.VisitorPanel.EmailAddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.VisitorPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Message)) { sb.Append(Resources.VisitorPanel.MessageRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
    public class ContactDetial_VM
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Address { get; set; }
        public HttpPostedFile Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContactTitle { get; set; }
        public string ContactDescription { get; set; }
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

            if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.SuperAdminPanel.EmailRequired); vm.Valid = false; }
            else if (!EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired + $"({Email})"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ContactNumber1)) { sb.Append(Resources.SuperAdminPanel.ContactNumberRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.SuperAdminPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.SuperAdminPanel.DescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address)) { sb.Append(Resources.SuperAdminPanel.AddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ContactTitle)) { sb.Append(Resources.SuperAdminPanel.ContactTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ContactDescription)) { sb.Append(Resources.SuperAdminPanel.ContactDescriptionRequired); vm.Valid = false; }
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
}