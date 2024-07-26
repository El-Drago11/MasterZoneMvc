using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class UserLoginPermissions
    {
        [ForeignKey(nameof(Permission))]
        public long PermissionId { get; set; }
        public Permission Permission { get; set; }

        [ForeignKey(nameof(UserLogin))]
        public long UserLoginId { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}