using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ValidateUserLogin_VM
    {
        public ApiResponse_VM ApiResponse_VM { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessAdminLoginId { get; set; }
        public long SuperAdminLoginId { get; set; }
        public long SubAdminLoginId { get; set; }

        public string UserRoleName { get; set; }
    }
}