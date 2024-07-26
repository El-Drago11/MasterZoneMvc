using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class StateMasters
    {

        [Key]
        public long ID { get; set; }
        public long CountryID { get; set; }
        public string Name { get; set; }
        public string StateCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
    }
}