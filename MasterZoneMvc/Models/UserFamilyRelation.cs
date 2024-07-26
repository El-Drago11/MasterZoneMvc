using System;
using System.ComponentModel.DataAnnotations;

namespace MasterZoneMvc.Models
{
    public class UserFamilyRelation
    {
        [Key]
        public long Id { get; set; }
        public long User1LoginId { get; set; }
        public long User2LoginId { get; set; }
        public string User1Relation_FieldTypeCatalogKey { get; set; } // User1 Relation with User2
        public string User2Relation_FieldTypeCatalogKey { get; set; } // User2 Relation with User1

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }

}