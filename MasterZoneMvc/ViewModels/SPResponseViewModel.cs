using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SPResponseViewModel
    {
        public int ret { get; set; }
        public string responseMessage { get; set; }
        public long Id { get; set; }
        public string resourceFileName { get; set; }
        public string resourceKey { get; set; }
    }

    public class SPResponseViewModel_UserAddUpdate : SPResponseViewModel
    {
        public string MasterId { get; set; }
    }
}