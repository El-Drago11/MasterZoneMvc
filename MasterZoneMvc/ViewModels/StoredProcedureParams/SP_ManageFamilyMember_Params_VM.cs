namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_ManageFamilyMember_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public int Mode { get; set; }

        public SP_ManageFamilyMember_Params_VM()
        {

        }
    }
}