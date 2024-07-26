using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateClassInterMediateDetail_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateClassInterMediateDetail_Param_VM()
        {
            Title = String.Empty;
            SubTitle = String.Empty;
            Description = String.Empty;
        }
    }
}
