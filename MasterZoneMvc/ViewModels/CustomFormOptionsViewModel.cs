using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CustomFormOptionsViewModel
    {
        public long Id { get; set; }
        public long CustomFormElementId { get; set; }
        public string CustomFormElementOptions { get; set; }
        public int Mode { get; set; }
    }
}