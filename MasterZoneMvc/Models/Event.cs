using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Event
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime_24HF { get; set; }
        public string EndTime_24HF { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int IsPaid { get; set; }
        public decimal Price { get; set; }
        public int TotalJoined { get; set; }
        public string EventLocationURL { get; set; }
        public string ShortDescription { get; set; }
        public string AboutEvent { get; set; }
        public string AdditionalInformation { get; set; }
        public string FeaturedImage { get; set; }
        public string TicketInformation { get; set; }
        public int Walkings { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ShowOnHomePage { get; set; }
        public long EventCategoryId { get; set; }
    }
}