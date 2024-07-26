using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Class
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        //public long InstructorLoginId { get; set; } // to remove
        public string Name { get; set; }
        public string Description { get; set; }
        //public int StudentMaxStrength { get; set; } // to remove
        public string ClassMode { get; set; }
        public decimal Price { get; set; }
        //public string ScheduledStartOnTime_24HF { get; set; } // to remove
        //public string ScheduledEndOnTime_24HF { get; set; } // to remove
        //public DateTime ScheduledOnDateTime { get; set; } // to remove
        public string ClassDays { get; set; }
        public string OnlineClassLink { get; set; }

        // Location if offline
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }


        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public int IsPaid { get; set; }
        public string ClassPriceType { get; set; }
        public string ClassURLLinkPassword { get; set; }

        //public int ClassDurationSeconds { get; set; } // to remove
        public string ClassType { get; set; }
        //public long GroupId { get; set; } // to remove
        public string ClassImage { get; set; }
        public string HowToBookText { get; set; }
        public string ClassDays_ShortForm { get; set; }

        public long ClassCategoryTypeId { get; set; }
        public int ShowOnHomePage { get; set; }
    }
}