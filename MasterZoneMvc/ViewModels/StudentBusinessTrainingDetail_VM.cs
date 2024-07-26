using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StudentBusinessTrainingDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogo { get; set; }
        public string BusinessLogoImageWithPath { get; set; }
        public string TrainingImageWithPath { get; set; }
        
    }
}