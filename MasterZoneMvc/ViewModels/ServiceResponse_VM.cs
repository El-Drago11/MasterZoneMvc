using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ServiceResponse_VM
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class ServiceResponse_VM<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}