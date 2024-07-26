using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateMasterpro_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Age { get; set; }
        public string Nationality { get; set; }
        public string Freelance { get; set; }
        public string Skype { get; set; }
        public string UploadCV { get; set; }
        public string Languages { get; set; }
        public long BusinessOwnerLoginId { get; set; }

        public int Mode { get; set; }



        public SP_InsertUpdateMasterpro_Params_VM()
        {
            Age = string.Empty;
            Nationality = string.Empty;
            Freelance = string.Empty;
            Skype = string.Empty;
            Languages = string.Empty;
            UploadCV = string.Empty;


        }
    }
}