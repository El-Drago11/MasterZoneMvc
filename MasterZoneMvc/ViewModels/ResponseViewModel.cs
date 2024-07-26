using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ResponseViewModel
    {
    }

    public class JqueryDataTable_Pagination_Response_VM<T>
    {
        public int draw { get; set; }
        public List<T> data { get; set; }
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }
        public object additionalData { get; set; }
    }
}