using System;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateUserFamilyRelation_Params_VM
    {
        public long Id { get; set; }
        public long User1LoginId { get; set; }
        public long User2LoginId { get; set; }
        public string User1Relation_FieldTypeCatalogKey { get; set; }
        public string User2Relation_FieldTypeCatalogKey { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateUserFamilyRelation_Params_VM()
        {
            User1Relation_FieldTypeCatalogKey = String.Empty;
            User2Relation_FieldTypeCatalogKey = String.Empty;
        }
    }
}