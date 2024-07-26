using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserViewModel
    {
    }

    public class UserBasicDetail_VM
    {
        public Int64 Id { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string Email { get; set; }
        public long RoleId { get; set; }
    }
}