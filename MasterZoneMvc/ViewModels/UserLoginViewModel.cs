using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserLoginViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public int Status { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public string Token { get; set; }
        public string MasterId { get; set; }
        public string ResetPasswordToken { get; set; }
        public int EmailConfirmed { get; set; }
    }


    public class UserLogin_VM
    {
        public Int64 Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryPhoneCode_Only { get; set; }
        public string PhoneNumber_Only { get; set; }
        public string Password { get; set; }
        public Int64 UserTypeId { get; set; } //Admin, Staff or Student
        public int LoginStatus { get; set; }
        public int IsDefaultPassword { get; set; }
        public string ResetPasswordToken { get; set; }
        public int EmailConfirmed { get; set; }
    }

    public class ManageUserLogin_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryPhoneCode_Only { get; set; }
        public string PhoneNumber_Only { get; set; }
        public string Password { get; set; }
        public Int64 UserTypeId { get; set; } //Admin, Staff or Student
        public int LoginStatus { get; set; }
        public int IsDefaultPassword { get; set; }
        public string ResetPasswordToken { get; set; }
        public int Mode { get; set; }

        public ManageUserLogin_SQL_Params_VM()
        {
            Username = "";
            Email = "";
            PhoneNumber = "";
            CountryPhoneCode_Only = "";
            PhoneNumber_Only = "";
            Password = "";
            ResetPasswordToken = "";
        }
    }
}