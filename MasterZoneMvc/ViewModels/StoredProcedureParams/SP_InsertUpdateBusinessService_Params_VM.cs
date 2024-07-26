using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessService_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FeaturedImage { get; set; }
        public string Icon { get; set; }
        public int SubmittedByLoginId { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public int ServiceType { get; set; }
        public SP_InsertUpdateBusinessService_Params_VM()
        {
            Title = string.Empty;
            Description = string.Empty;
            FeaturedImage = string.Empty;
            Icon = string.Empty;
        }
    }
}