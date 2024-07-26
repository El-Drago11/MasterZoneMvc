using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateMenu_Params_VM
    {
        public long Id { get; set; }
        public long ParentMenuId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string PageLink { get; set; }
        public int IsActive { get; set; }
        public int IsShowOnHomePage { get; set; }
        public string Tag { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public int SortOrder { get; set; }

        public SP_InsertUpdateMenu_Params_VM()
        {
            Name = "";
            Image = "";
            PageLink = "";
            Tag = "";
        }
    }
}