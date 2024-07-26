using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class RegisterBusinessAdminViewModel
    {
        public long BusinessCategoryId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessName { get; set; }

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
            if (BusinessCategoryId <= 0) { sb.Append(Resources.BusinessPanel.SelectCategoryRequired); vm.Valid = false; }
            else if (BusinessSubCategoryId <= 0) { sb.Append(Resources.BusinessPanel.SelectSubCategoryRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(DOB)) { sb.Append(Resources.BusinessPanel.DateOfBirthRequired); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Email) && !EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Password) && !PasswordValidator.IsValidPassword(Password)) { sb.Append(PasswordValidator.PasswordValidationMessage); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.BusinessPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(PhoneNumber) && !PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(BusinessName)) { sb.Append(Resources.BusinessPanel.BusinessNameRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            } 

            return vm;
        }
    }

    public class RegisterBusiness_VM
    {
        public long Id { get; set; }
        public long BusinessCategoryId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int Mode { get; set; }
        public int Verified { get; set; }

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
            if (BusinessCategoryId <= 0) { sb.Append(Resources.BusinessPanel.InvalidBusinessCategory_ErrorMessage); vm.Valid = false; }
            else if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.BusinessPanel.FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.BusinessPanel.LastNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); vm.Valid = false; }
            else if (!EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired + $"({Email})"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.BusinessPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            else if (Mode == 3 && Verified <= 0)
            {
                 sb.Append(Resources.BusinessPanel.SelectVerifiedRequired); vm.Valid = false;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

}