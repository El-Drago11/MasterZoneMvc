using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessBranch_Param_VM
    {

        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long BranchBusinessLoginId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessBranch_Param_VM()
        {
            Name = string.Empty;
        }
    }
}