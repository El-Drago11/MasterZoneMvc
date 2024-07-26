using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_GetRecordByMenu_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string MenuTag { get; set; }
        public string CategoryKey { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }
        public string ItemType { get; set; }
        public string City { get; set; }
        public int Mode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string CategorySearch { get; set; }
        public string SearchType { get; set; }
        public string SearchValue { get; set; }

        public SP_GetRecordByMenu_Params_VM()
        {
            MenuTag = string.Empty;
            CategoryKey = string.Empty;
            ItemType = string.Empty;
            City= string.Empty;
            CategorySearch = string.Empty;
            SearchType = string.Empty;
            SearchValue = string.Empty;
        }

    }
}