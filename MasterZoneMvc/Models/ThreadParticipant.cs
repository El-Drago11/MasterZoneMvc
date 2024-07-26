using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ThreadParticipant
    {
        [Key]
        public long Id { get; set; }

        //[ForeignKey(nameof(ChatThread))]

        public long ChatThreadId { get; set; }
        //public ChatThread ChatThread { get; set; }


        //[ForeignKey(nameof(UserLogin))]

        public long UserLoginId { get; set; }
        //public UserLogin UserLogin { get; set; }
    }
}