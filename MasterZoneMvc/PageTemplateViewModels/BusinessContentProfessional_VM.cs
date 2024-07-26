using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentProfessional_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string ProfessionalTitle { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
    }
    public class BusinessContentProfessionalDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string ProfessionalTitle { get; set; }
        public string Description { get; set; }
    }
}