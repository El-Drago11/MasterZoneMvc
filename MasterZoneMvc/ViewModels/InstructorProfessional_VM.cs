using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class InstructorProfessional_VM
    {
        public long Id { get; set; }
        public string BusinessName { get; set; }
        public string Designation { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public  string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }
        public long InstructorUserLoginId { get; set; }
    }
}