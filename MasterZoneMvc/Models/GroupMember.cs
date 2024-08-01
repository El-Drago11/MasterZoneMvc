using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class GroupMember
    {
        [Key]
        public long Id { get; set; }

        public long GroupId { get; set; }

        public long UserLoginId { get; set; }
    }
}