using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class JqueryDataTableParamsViewModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string searchValue { get; set; }
        public int sortColumnIndex { get; set; }
        public string sortColumn { get; set; }
        public string sortOrder { get; set; }
        public string sortDirection { get; set; }
        public int recordsTotal { get; set; }
    }
}