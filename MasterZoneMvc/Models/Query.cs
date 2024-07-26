using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class Query
    {
        [Key]
        public long Id { get; set; }

        public long StudentId { get; set; }

        public long BusinessOwnerId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int IsReplied { get; set; }
        public string ReplyBody { get; set; }
        public DateTime RepliedOn { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}