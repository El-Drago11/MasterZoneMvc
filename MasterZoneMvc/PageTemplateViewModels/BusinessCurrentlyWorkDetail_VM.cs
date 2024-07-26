using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessCurrentlyWorkDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessOwnerName { get; set; }
        public string BusinessLogoImage { get; set; }
        public string BusinessName { get; set; }
        public int Mode { get; set; }
    }
}