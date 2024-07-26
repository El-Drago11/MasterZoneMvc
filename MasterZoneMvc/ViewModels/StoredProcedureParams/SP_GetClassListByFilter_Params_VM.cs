namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_GetClassListByFilter_Params_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string SearchValue { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public string ClassMode { get; set; }
        public int Mode { get; set; }

        public SP_GetClassListByFilter_Params_VM()
        {
            SearchValue = string.Empty;
            ClassMode = string.Empty;
        }
    }
}