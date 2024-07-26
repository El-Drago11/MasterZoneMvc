using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class SportsBookingCheaqueDetail
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long SlotId { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BookedId { get; set; }
        public string Department { get; set; }
        public string Apartment { get; set; }
        public string HouseNumber { get; set; }
        public string Message { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public string TennisTitle { get; set; }
        public string TennisImage { get; set; }
        public string RoomTime { get; set; }
        public string SelectDate { get; set; }
        public string RoomService { get; set; }
        public int PlayerCount { get; set; }
        public int Request { get; set; }

    }
}