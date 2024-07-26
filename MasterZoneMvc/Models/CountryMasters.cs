using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class CountryMasters
    {

        [Key]
        public long ID { get; set; }

        public string Name { get; set; }
        public string CountryCode { get; set; }

        public string ISO2 { get; set; }

        public string ISO3 { get; set; }

        public string NumericCode { get; set; }

        public string PhoneCode { get; set; }

        public string PhoneCodeWithPlus { get; set; }

        public string Capital { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySymbol { get; set; }

        public string TLD { get; set; }

        public string Native { get; set; }

        public string Region { get; set; }

        public string Subregion { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int Status { get; set; }
    }
}