using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateEnquiriesFollowsup_Params_VM
    {
        public long Id { get; set; }
        public long EnquiryId  { get; set; }
        public long FollowedbyLoginId { get; set; }
        public long SubmittedByLoginId { get; set; }
        public string Comments { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateEnquiriesFollowsup_Params_VM()
        {
            Comments = String.Empty;
        }
    }
}