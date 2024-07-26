namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateFamilyMember_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relation { get; set; }
        public string ProfileImage { get; set; }
        public int Gender { get; set; }
        public int Mode { get; set; }

        public SP_InsertUpdateFamilyMember_Params_VM()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Relation = string.Empty;
            ProfileImage = string.Empty;
        }
    }
}