using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class DeleteFile_SP_ManageBusiness_Get_Params_VM_Delete
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId   { get; set; }
        public int Mode { get; set; }
    }
}