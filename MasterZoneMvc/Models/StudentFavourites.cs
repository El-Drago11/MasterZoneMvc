using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class StudentFavourites
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey("StudentUserLogin")]
        public long StudentLoginId { get; set; }
        //public UserLogin StudentUserLogin { get; set; }

        //[ForeignKey("FavouriteUserLogin")]
        public long FavouriteUserLoginId { get; set; }
        //public UserLogin FavouriteUserLogin { get; set; }
    }
}