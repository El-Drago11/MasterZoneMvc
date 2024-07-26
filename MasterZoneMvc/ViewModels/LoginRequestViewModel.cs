using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class LoginRequestViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberLogin { get; set; }

    }
}