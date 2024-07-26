﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateClassicDanceProfileDetail_Paarm_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ClassicImage { get; set; }
        public string Image { get; set; }
        public string ScheduleImage { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
    }
}