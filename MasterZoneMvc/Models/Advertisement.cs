using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Advertisement
    {
        [Key]
        public long Id { get; set; }
        public string AdvertisementCategory { get; set; } // GYM, YOGA etc.. 
        public string Image { get; set; }
        public int Status { get; set; }

        //public string ImageAspectRatio { get; set; } // [1:1], [4:3]
        //public string ImageResolution { get; set; } // 1920x1080
        public string ImageOrientationType { get; set; } // horizontal or vertical

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public long CreatedForLoginId { get; set; }
        public string AdvertisementLink { get; set; }
    }
}