using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessTrainingDetail_VM
    {
        public long Id { get;set;}
        public string TrainingName { get;set;}
        public string Duration { get;set;}
        public string TrainingClassDays { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string FormattedDuration { get; set; }
        public int Mode { get; set; }

    }
}