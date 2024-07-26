using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class PermissionViewModel
    {
        public long Id { get; set; }
        public long ParentPermissionId { get; set; }
        public string KeyName { get; set; }
        public string TextValue { get; set; }
        //public string Comments { get; set; }
        public long PanelTypeId { get; set; } // SuperAdmin, Business etc.
    }

    public class PermissionHierarchy_VM : PermissionViewModel
    {
        public List<PermissionHierarchy_VM> SubPermissions { get; set; }
    }
}