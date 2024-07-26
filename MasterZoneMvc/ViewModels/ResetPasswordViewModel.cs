using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required]
        public string Token { get; set; } // Encrypted User Id

        [Required(ErrorMessage = "Please enter new password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please re-enter new password")]
        [Compare("Password", ErrorMessage = "Password's doesn't matched!")]
        public string ConfirmPassword { get; set; }
    }
    public class ResetPasswordToken_VM
    {
        public Int64 UserId { get; set; }
        public DateTime ValidTill_UTCDateTime { get; set; }
    }

    public class ResetPassword_VM
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

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
            if (String.IsNullOrEmpty(Token.Trim())) { sb.Append("Invalid Token!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Password.Trim())) { sb.Append(Resources.VisitorPanel.PasswordRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}