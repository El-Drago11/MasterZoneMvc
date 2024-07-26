using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class ClassBatch
    {
        [ForeignKey(nameof(Batch))]
        public long BatchId { get; set; }
        public Batch Batch { get; set; }

        [ForeignKey(nameof(Class))]
        public long ClassId { get; set; }
        public Class Class { get; set; }
    }
}