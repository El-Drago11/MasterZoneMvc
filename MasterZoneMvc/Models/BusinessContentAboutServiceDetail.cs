using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessContentAboutServiceDetail
    {
        [Key]
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string AboutServiceTitle { get; set; }
        public string AboutServiceDescription { get; set; }
        public string AboutServiceIcon { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }


    }
}