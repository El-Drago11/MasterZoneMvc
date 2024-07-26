using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ThreadMessage
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(ChatThread))]
        public long ChatThreadId { get; set; }
        //public ChatThread ChatThread { get; set; }


        //[ForeignKey(nameof(UserLogin))]
        public long SenderUserLoginId { get; set; }
        //public UserLogin UserLogin { get; set; }

        public string MessageBody { get; set; }

        public DateTime CreatedOn { get; set; }

        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}