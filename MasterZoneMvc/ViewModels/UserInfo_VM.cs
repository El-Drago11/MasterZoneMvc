using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserInfo_VM
    {
        public string id { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string email { get; set; }
        public string token { get; set; }

    }

    public class FacebookInfo_VM
    {
        public string id { get; set; }
        public Name name { get; set; }
        public string email { get; set; }
    }

    public class Name
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

}