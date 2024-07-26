using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SearchByMenu_APIParmas_VM
    {
        public string MenuTag { get; set; }
        public string CategorySearchValue { get; set; }
        public string City { get; set; }
        public long LastRecordId { get; set; }
        public string RecordLimit { get; set; }
        public string SearchType { get; set; }
        public string SearchValue { get; set; }

        public SearchByMenu_APIParmas_VM()
        {
            City = string.Empty;
            CategorySearchValue = string.Empty;
        }
    }
}