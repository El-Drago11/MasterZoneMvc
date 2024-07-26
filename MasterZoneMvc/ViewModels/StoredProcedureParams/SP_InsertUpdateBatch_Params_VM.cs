using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{

    public class SP_InsertUpdateBatch_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string StartTime24HF { get; set; }
        public string EndTime24HF { get; set; }
        public int StudentMaxStrength { get; set; }
        public long InstructorLoginId { get; set; }
        public long GroupId { get; set; }
        public int ClassDuration { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public int Status { get; set; }

        public SP_InsertUpdateBatch_Params_VM()
        {
            Name = string.Empty;
            StartTime24HF = string.Empty;
            EndTime24HF = string.Empty;
        }
    }
}