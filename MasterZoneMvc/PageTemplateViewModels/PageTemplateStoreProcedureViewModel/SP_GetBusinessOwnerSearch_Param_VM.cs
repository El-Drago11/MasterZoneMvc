using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_GetBusinessOwnerSearch_Param_VM
    {

        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long CertificateId { get; set; }
        public long BusinessCategoryId { get; set; }
        public string SearchKeyword { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public int Mode { get; set; }
        public SP_GetBusinessOwnerSearch_Param_VM ()
        {
            Id = 0;
            UserLoginId = 0;
            BusinessCategoryId = 0;
            SearchKeyword = string.Empty;
            LastRecordId = 0;
            RecordLimit = 10;
        }
    }
}