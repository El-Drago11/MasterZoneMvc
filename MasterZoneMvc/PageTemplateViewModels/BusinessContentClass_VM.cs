using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentClass_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
    }
    public class BusinessContentClassDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ClassCategoryDetailList_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string CategoryName { get; set; }
        public int Mode { get; set; }
    }
}