using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContactDetail_VM
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string FacebookProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
    }
}