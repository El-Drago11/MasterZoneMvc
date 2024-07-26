using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class FieldTypeCatalog
    {
        [Key]
        public long Id { get; set; }
        public long ParentId { get; set; }
        public long PanelTypeId { get; set; }
        public string KeyName { get; set; }
        public string TextValue { get; set; }
        public int IsActive { get; set; }
    }
}