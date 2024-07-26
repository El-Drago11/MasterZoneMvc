﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentWorldClassProgram_Param_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Options { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentWorldClassProgram_Param_VM()
        {
            Title = string.Empty;
            Image = string.Empty;
            Options = string.Empty;
        }
    }
}