using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessContentVideoCategoryViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class BusinessContentVideoCategory_VM
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int IsActive { get; set; }
    }
}