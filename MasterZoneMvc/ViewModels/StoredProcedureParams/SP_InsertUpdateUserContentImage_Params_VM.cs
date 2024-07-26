using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateUserContentImage_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public string ImageThumbnail { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateUserContentImage_Params_VM()
        {
            ImageTitle = String.Empty;
            Image = String.Empty;
            ImageThumbnail = String.Empty;
            Mode = 0;
        }
    }
}