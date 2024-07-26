using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ApartmentBooking
    {
        [Key]
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        //public string BlockName { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        //public string AreaName { get; set; }
        public string Activity { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public long ApartmentAreaId { get; set; }
        public long ApartmentBlockId { get; set; }
    }
}