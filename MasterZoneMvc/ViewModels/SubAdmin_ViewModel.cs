using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class SubAdmin_ViewModel
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public HttpPostedFile ProfileImage { get; set; }

        // Location if offline
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string PermissionIds { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.ValidInformationMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.SuperAdminPanel.SubAdmin_FirstNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.SuperAdminPanel.SubAdmin_LastNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.SuperAdminPanel.EmailRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(Email) && !EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Password)) { sb.Append(Resources.SuperAdminPanel.PasswordRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PermissionIds)) { sb.Append(Resources.SuperAdminPanel.SubAdmin_PermisionsRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Address))
            {
                sb.Append(Resources.SuperAdminPanel.SubAdmin_LocationRequired);
                vm.Valid = false;
            }
            else if (String.IsNullOrEmpty(State))
            {
                sb.Append(Resources.SuperAdminPanel.SubAdmin_StateRequired);
                vm.Valid = false;
            }
            else if (String.IsNullOrEmpty(Country))
            {
                sb.Append(Resources.SuperAdminPanel.SubAdmin_CountryRequired);
                vm.Valid = false;
            }
            else if (String.IsNullOrEmpty(City))
            {
                sb.Append(Resources.SuperAdminPanel.SubAdmin_CityRequired);
                vm.Valid = false;
            }
            else if (String.IsNullOrEmpty(Pincode))
            {
                sb.Append(Resources.SuperAdminPanel.SubAdmin_PincodeRequired);
                vm.Valid = false;
            }
            else if (String.IsNullOrEmpty(LandMark))
            {
                sb.Append(Resources.SuperAdminPanel.SubAdmin_LandmarkRequired);
                vm.Valid = false;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }


    public class SubAdminDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }

        // Location if offline
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Pincode { get; set; }

    }


    public class SubAdminEdit_VM : SubAdminDetail_VM
    {
    
        public List<PermissionHierarchy_VM> Permissions { get; set; }
    }

    public class SubAdmin_Pagination_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string ProfileImage { get; set; }

        // Location if offline
        public string ProfileImageWithPath { get; set; }
        public string Address { get; set; }
        //public string Pincode { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }


    public class SubAdmin_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class SubAdminDetailByUser_VM
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
    }
}