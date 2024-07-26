using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EmailSender_VM
    {
        public string ToEmail { get; set; }
        public string ReceiverName { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string AttachFileName { get; set; }
    }
}