using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessStaffViewModel
    {
        public long StaffCategoryId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string StaffName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Error_VM ValidInformation()
        {

            Error_VM response = new Error_VM();
            response.Valid = true;

            if (this == null)
            {
                response.Valid = false;
                response.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return response;
            }

            StringBuilder sb = new StringBuilder();
            if (StaffCategoryId <= 0) { sb.Append(Resources.BusinessPanel.BusinessCategoryRequired); response.Valid = false; }
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.NameRequired); response.Valid = false; }
            if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.EmailRequired); response.Valid = false; }
            if (String.IsNullOrEmpty(StaffName)) { sb.Append(Resources.BusinessPanel.StaffNameRequired); response.Valid = false; }
            if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.BusinessPanel.PasswordRequired); response.Valid = false; }
            if (String.IsNullOrEmpty(ConfirmPassword)) { sb.Append(Resources.BusinessPanel.ConfirmPasswordRequired); response.Valid = false; }

            //if (Files.Count() == 0) { sb.Append("Please attach business documents!"); response.Valid = false; }

            // if all are valid then return success response
            if (response.Valid == false)
            {
                response.Message = sb.ToString();
            }

            return response;
        }
    
        
    }
}