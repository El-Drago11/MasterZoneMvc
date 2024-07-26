using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CountryViewModel
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string PhoneCode { get; set; }
        public int Status { get; set; }
    }
}