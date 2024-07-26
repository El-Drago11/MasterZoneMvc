using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessContentServicesViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ServiceTitle { get; set; }
        public string ShortDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
    }

    public class BusinessContentServices_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ServiceTitle { get; set; }
        public string ShortDescription { get; set; }
    }
}