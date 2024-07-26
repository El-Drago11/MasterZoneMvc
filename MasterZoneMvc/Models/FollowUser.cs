using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class FollowUser
    {
        [Key]
        public long Id { get; set; }
        public long FollowerUserLoginId { get; set; }
        public long FollowingUserLoginId { get; set; }
    }
}