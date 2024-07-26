using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentCourceCategory_PPCMeta_Param_VM
    {

        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long CourseCategoryId { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentCourseCategoryDetail_PPCMeta
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long CourseCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseCategoryImage { get; set; }
        public string CourseCategoryImageWithPath { get; set; }

    }
   

    
}