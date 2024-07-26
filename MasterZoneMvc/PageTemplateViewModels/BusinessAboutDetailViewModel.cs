using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessAboutDetailViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string About { get; set; }
        public string AboutImage { get; set; }
        public string AboutImageWithPath { get; set; }
        public string HowToBookText { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessName { get; set; }
    }

}