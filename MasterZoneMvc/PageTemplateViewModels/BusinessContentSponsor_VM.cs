using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentSponsor_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string SponsorTitle { get; set; }
        public string SponsorIcon { get; set; }
        public string BusinessSponsorImageWithPath { get; set; }
        public string SponsorLink { get; set; }
        public int Mode { get; set; }
    }
}