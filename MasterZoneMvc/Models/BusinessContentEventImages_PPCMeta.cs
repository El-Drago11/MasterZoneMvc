using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessContentEventImages_PPCMeta
    {
        [Key]
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long EventId { get; set; }
        public string Image { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

    }
}