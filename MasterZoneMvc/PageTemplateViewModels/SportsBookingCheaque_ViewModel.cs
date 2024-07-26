using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class SportsBookingCheaque_ViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public long SlotId { get; set; }
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
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }

            StringBuilder sb = new StringBuilder();

            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.VisitorPanel.NameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(BookedId)) { sb.Append(Resources.VisitorPanel.BookedIdRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(SurName)) { sb.Append(Resources.VisitorPanel.SurNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Department)) { sb.Append(Resources.VisitorPanel.DepartmentRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Apartment)) { sb.Append(Resources.VisitorPanel.ApartmentRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.VisitorPanel.EmailAddressRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PhoneNumber)) { sb.Append(Resources.VisitorPanel.PhoneNumberRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Message)) { sb.Append(Resources.VisitorPanel.MessageRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class SportsBookingCheaqueDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BookedId { get; set; }
        public string Department { get; set; }
        public string Apartment { get; set; }
        public string HouseNumber { get; set; }
        public string Message { get; set; }
        public int Mode { get; set; }
    }
}