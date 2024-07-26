using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Menu
    {
        [Key]
        public long Id { get; set; }
        public long ParentMenuId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string PageLink { get; set; }
        public int IsActive { get; set; }
        public string Tag { get; set; }
        public int SortOrder { get; set; }
        public int IsShowOnHomePage { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

    }
}