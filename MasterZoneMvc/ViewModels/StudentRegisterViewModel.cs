using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StudentRegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public int Gender { get; set; }
        public string OTP { get; set; }

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
            if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.VisitorPanel.EmailRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Email) && !EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.VisitorPanel.PasswordRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Password) && !PasswordValidator.IsValidPassword(Password)) { sb.Append(PasswordValidator.PasswordValidationMessage); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.VisitorPanel.FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.VisitorPanel.LastNameRequired); vm.Valid = false; }
            
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.VisitorPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(PhoneNumber) && !PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            
            else if (Gender <= 0) { sb.Append(Resources.VisitorPanel.GenderRequired); vm.Valid = false; }
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class UserListStudents_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public long UserLoginId { get; set; }
    }

    public class BusinessStudentRegisterViewModel
    {
        public string MasterId { get; set; }
        public int HasMasterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public int Gender { get; set; }
        public HttpPostedFile BusinessStudentProfileImage { get; set; }

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
            if(HasMasterId == 1)
            {
                if (String.IsNullOrEmpty(MasterId)) { sb.Append(Resources.VisitorPanel.MasterId_Required); vm.Valid = false; }
            }
            else
            {
                if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.VisitorPanel.EmailRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.VisitorPanel.PasswordRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.VisitorPanel.FirstNameRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.VisitorPanel.LastNameRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.VisitorPanel.PhoneNumberRequired); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(PhoneNumber)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
                else if (Gender <= 0) { sb.Append(Resources.VisitorPanel.GenderRequired); vm.Valid = false; }
            }
            
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }


        public class VerifyOtpViewModel
        {
            public string OTP { get; set; }
            public string Email { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}