using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StateViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long CountryID { get; set; }
        public string StateCode { get; set; }
        public string StateCode_TwoDigits { get; set; }
        public string StateCode_ThreeDigits { get; set; }

        public string PhoneCode { get; set; }
    }
}