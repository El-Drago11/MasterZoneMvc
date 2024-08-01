using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class SP_ManageTennisTiming_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public long slotId { get; set; }
        public string BookDate { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Mode { get; set; }
    }
}
