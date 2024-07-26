using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SuperAdminDashboardDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long TotalAdvertisements { get; set; }
        public long TotalCertificates { get; set; }
        public long TotalBusiness { get; set; }
        public long TotalStudent { get; set; }
        public long TotalSubAdmin { get; set; }
        public long TotalPlans { get; set; }
    }
}