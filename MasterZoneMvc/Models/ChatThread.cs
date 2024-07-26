using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ChatThread
    {
        [Key]
        public long Id { get; set; }

        public int ThreadType { get; set; } // One-to-One Chat(1) or Group(2)

        public long CreatedByUserLoginId { get; set; }

        //public IEnumerable<ThreadParticipant> ThreadParticipants { get; set; }

        public DateTime CreatedOn { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}