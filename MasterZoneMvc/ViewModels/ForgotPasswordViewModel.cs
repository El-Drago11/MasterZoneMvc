using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string Email { get; set; }
        public string ReturnUrl { get; set; }


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
            if (String.IsNullOrEmpty(Email.Trim())) { sb.Append(Resources.VisitorPanel.EmailRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class User_VM
    {
        public long Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public int EmailConfirmed { get; set; }

        //[ForeignKey("Role")]
        public long RoleId { get; set; }
        public string MasterId { get; set; }

        public string GoogleUserId { get; set; }
        public string FacebookUserId { get; set; }
        public string GoogleAccessToken { get; set; }
        public string FacebookAccessToken { get; set; }

        public string UniqueUserId { get; set; }

        public string ResetPasswordToken { get; set; }

    }
}