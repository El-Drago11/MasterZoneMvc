using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateSportsBookingCheaque_Param_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long SlotId { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BookedId { get; set; }
        public string Department { get; set; }
        public string Apartment { get; set; }
        public string HouseNumber { get; set; }
        public string Message { get; set; }
        public string RoomTime { get; set; }
        public string RoomService { get; set; }
        public string SelectDate { get; set; }
        public string TennisTitle { get; set; }
        public string TennisImage { get; set; }
        public int PlayerCount { get; set; }
        public int Request { get; set; }
        public decimal Price { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateSportsBookingCheaque_Param_VM ()
        {
            Name = string.Empty;
            SurName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;                
            BookedId = string.Empty;
            Department = string.Empty;
            Apartment = string.Empty;
            HouseNumber = string.Empty;
            Message = string.Empty;
            RoomTime = string.Empty;
            RoomService = string.Empty;
            SelectDate = string.Empty;
            TennisTitle = string.Empty;
            TennisImage = string.Empty;
            SubmittedByLoginId = 0;

        }
    }
}