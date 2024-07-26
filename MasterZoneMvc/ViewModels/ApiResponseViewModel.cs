using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ApiResponseViewModel<T>
    {
        public int status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class ApiResponse_VM
    {
        public int status { get; set; }
        public long Id { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}