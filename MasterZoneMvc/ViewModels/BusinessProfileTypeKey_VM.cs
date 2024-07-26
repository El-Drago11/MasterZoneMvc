using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessProfileTypeKey_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Key { get; set; }
        public int ProfilePageTypeId { get; set; }
        public string Name { get; set; }
    }
}