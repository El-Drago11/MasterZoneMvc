using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class GroupMessage
    {
        [Key]
        public long Id { get; set; }
        public long SenderUserloginId { get; set; }
        public long ReceiverUserLoginId { get; set; }
        public string Messagebody { get; set; }
        public int SenderStatus { get; set; } 
        public int ReceiverStatus { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        [ForeignKey(nameof(Group))]
        public long GroupId { get; set; }
        public Group Group { get; set; }


    }
}