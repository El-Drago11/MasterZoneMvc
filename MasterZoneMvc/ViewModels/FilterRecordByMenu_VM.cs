using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class FilterRecordByMenu_VM
    {
        public string MenuTag { get; set; }
        public string City { get; set; }
        public string CategorySearchValue { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public long UserLoginId { get; set; }
        public string SearchType { get; set; }
        public string SearchValue { get; set; }
    }
}