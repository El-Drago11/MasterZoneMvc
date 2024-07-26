namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_GetRecordsBySearch_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string MenuTag { get; set; }
        public string Location { get; set; } // name/(current for lat-long)
        public string Latitude { get; set; } // passed if used by current location
        public string Longitude { get; set; }
        public string ItemType { get; set; } // academies/classes/instructors/events/trainings
        public string SearchText { get; set; } // name
        public string ItemMode { get; set; } // online/offline
        public string PriceType { get; set; } // free/paid
        public string StartDate { get; set; }
        public string Days { get; set; }
        public long CategoryId { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int Mode { get; set; }
        public SP_GetRecordsBySearch_Params_VM()
        {
            MenuTag = string.Empty;
            Location = string.Empty;
            Latitude = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;
            SearchText = string.Empty;
            ItemType = string.Empty;
            ItemMode = string.Empty;
            PriceType = string.Empty;
            StartDate = string.Empty;
            Days = string.Empty;
        }
    }
}