using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserFamilyRelationViewModel
    {
        public long Id { get; set; }
        public long User1LoginId { get; set; }
        public long User2LoginId { get; set; }
        public string User1Relation_FieldTypeCatalogKey { get; set; }
        public string User2Relation_FieldTypeCatalogKey { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }

    public class UserRelationManagement_VM
    {
        public string User1_FamilyRelationKey { get; set; }
        public int User1Gender { get; set; }
        public string User2_FamilyRelationKey { get; set; }
    }

    public class UserFamilyMemberRelation_VM
    {
        public long Id { get; set; }
        public long User1LoginId { get; set; }
        public long User2LoginId { get; set; }
        public string User1Relation_FieldTypeCatalogKey { get; set; }
        public string User2Relation_FieldTypeCatalogKey { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string UserProfileImage { get; set; }
        public string UserProfileImageWithPath { get; set; }
        public string User2Relation_FieldTypeCatalogTextValue { get; set; }
    }
}