using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class LoginViewModel
    {

        public string Email
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public bool RememberLogin
        {
            get;
            set;
        }
        public string ReturnUrl
        {
            get;
            set;
        }

        public int DeviceType { get; set; } // Android/iOS
        public string DeviceToken { get; set; } // Device token needed for notifications

        public string MasterId { get; set; }
        public string SocialLoginId { get; set; }


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
            //if (String.IsNullOrEmpty(Email.Trim())) { sb.Append(Resources.VisitorPanel.EmailRequired); vm.Valid = false; }
            // if (String.IsNullOrEmpty(MasterId.Trim())) { sb.Append(Resources.ErrorMessage.Login_MasterIdRequired); vm.Valid = false; }
            // else if (String.IsNullOrEmpty(Password.Trim())) { sb.Append(Resources.VisitorPanel.PasswordRequired); vm.Valid = false; }
            if (string.IsNullOrEmpty(SocialLoginId))
            {
                // If SocialLoginId is empty, Password is required
                if (string.IsNullOrEmpty(Password?.Trim()))
                {
                    sb.Append(Resources.VisitorPanel.PasswordRequired);
                    vm.Valid = false;
                }
            }
            if (string.IsNullOrEmpty(SocialLoginId))
            {
                // If SocialLoginId is empty, Password is required
                if (string.IsNullOrEmpty(MasterId?.Trim()))
                {
                    sb.Append(Resources.ErrorMessage.Login_MasterIdRequired);
                    vm.Valid = false;
                }
            }

            

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }


    public class LoginService_VM
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int Mode { get; set; }
        public string SocialLoginId { get; set; }
        public string MasterId { get; set; }

    }

    public class Login_VM
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int Mode { get; set; }
        public string SocialMediaType { get; set; }
        public string FirstName { get; set; }
        public string SocialLoginId { get; set; }
        public string MasterId { get; set; }
        public int DeviceType { get; set; } // Android/iOS
        public string DeviceToken { get; set; } // Device token needed for notifications
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
            //if (String.IsNullOrEmpty(Email.Trim())) { sb.Append(Resources.VisitorPanel.EmailRequired); vm.Valid = false; }
            // if (String.IsNullOrEmpty(MasterId.Trim())) { sb.Append(Resources.ErrorMessage.Login_MasterIdRequired); vm.Valid = false; }
            // else if (String.IsNullOrEmpty(Password.Trim())) { sb.Append(Resources.VisitorPanel.PasswordRequired); vm.Valid = false; }
            if (string.IsNullOrEmpty(SocialLoginId))
            {
                // If SocialLoginId is empty, Password is required
                if (string.IsNullOrEmpty(Password?.Trim()))
                {
                    sb.Append(Resources.VisitorPanel.PasswordRequired);
                    vm.Valid = false;
                }
            }
            if (string.IsNullOrEmpty(SocialLoginId))
            {
                // If SocialLoginId is empty, Password is required
                if (string.IsNullOrEmpty(MasterId?.Trim()))
                {
                    sb.Append(Resources.ErrorMessage.Login_MasterIdRequired);
                    vm.Valid = false;
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