using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdatePaymentResponse_Params_VM
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string Provider { get; set; }
        public string ResponseStatus { get; set; }
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public int IsApproved { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        

        public SP_InsertUpdatePaymentResponse_Params_VM()
        {
            // Make default string value to empty
            Provider = "";
            ResponseStatus = "";
            TransactionID = "";
            Description = "";
            Method = "";
        }
    }
}