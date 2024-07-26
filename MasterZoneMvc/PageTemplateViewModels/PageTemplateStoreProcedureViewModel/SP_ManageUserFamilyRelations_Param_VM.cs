namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_ManageUserFamilyRelations_Param_VM
    {
        public long Id { get; set; }
        public string MasterId { get; set; }
        public long User1LoginId   { get; set; }
        public int Mode { get; set; }
        public SP_ManageUserFamilyRelations_Param_VM()
        {
            MasterId = string.Empty;
        }
    }
}