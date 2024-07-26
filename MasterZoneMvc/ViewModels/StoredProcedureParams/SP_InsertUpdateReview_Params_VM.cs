using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateReview_Params_VM
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public int Rating { get; set; }
        public string ReviewBody { get; set; }
        public long ReviewerUserLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateReview_Params_VM ()
        {
            ReviewBody = string.Empty;
            ItemType = string.Empty;
        }
    }
}