using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Permission
    {
        [Key]
        public long Id { get; set; }
        public long ParentPermissionId { get; set; }

        public string KeyName { get; set; }
        public string TextValue { get; set; }
        public string Comments { get; set; }
        public long PanelTypeId { get; set; } // SuperAdmin, Business etc.
    }
}