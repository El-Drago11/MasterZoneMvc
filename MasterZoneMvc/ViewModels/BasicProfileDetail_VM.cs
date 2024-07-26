using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BasicProfileDetail_VM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public int IsCertified { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }

    }
}