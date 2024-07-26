using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateUserContentVideo_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }      
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string VideoDescription { get; set; }
        public string VideoThumbnail { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateUserContentVideo_Params_VM ()
        {
            VideoTitle = String.Empty;
            VideoLink = String.Empty;
            VideoDescription = String.Empty;
            VideoThumbnail = String.Empty;
            Mode = 0;
        }
    }

}