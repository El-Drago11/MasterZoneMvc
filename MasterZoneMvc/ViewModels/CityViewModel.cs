using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CityViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long StateID { get; set; }
        public int Status { get; set; }
    }
}