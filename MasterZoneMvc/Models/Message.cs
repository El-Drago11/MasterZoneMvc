using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    public class Message
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
    }
}