using MasterZoneMvc.Common;
using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessSearchViewModel
    {

    }

    public class RequestBusinessSearchViewModel
    {
        public string SearchKeyword { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public long LastRecordId { get; set; }
        public long BusinessCategoryId { get; set; }
        public string SearchBy { get; set; }
        public int RecordLimit { get; set; }

        public RequestBusinessSearchViewModel()
        {
            RecordLimit = StaticResources.RecordLimit_Default;
        }

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
            if (String.IsNullOrEmpty(SearchBy)) { sb.Append(Resources.VisitorPanel.SearchValueRequired); vm.Valid = false; }
            else if (SearchBy.ToLower() == "category" && BusinessCategoryId <= 0) { sb.Append(Resources.VisitorPanel.CategoryIdRequired); vm.Valid = false; }
            else if (SearchBy.ToLower() == "name" && String.IsNullOrEmpty(SearchKeyword)) { sb.Append(Resources.VisitorPanel.SearchKeywordsRequired); vm.Valid = false; }
            else if (SearchBy.ToLower() == "currentlocation" && (String.IsNullOrEmpty(Latitude) || String.IsNullOrEmpty(Longitude))) { sb.Append(Resources.VisitorPanel.LatitudeLongitudeValuesRequired); vm.Valid = false; }
            else if (SearchBy.ToLower() == "location" && (String.IsNullOrEmpty(SearchKeyword))) { sb.Append(Resources.VisitorPanel.SearchKeywordsRequired); vm.Valid = false; }
            //if (Mode <= 0) { sb.AppendLine("Invalid Mode!"); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }
    }

    public class RequestBusinessSearch_ForSuperAdmin_VM
    {
        public long CertificateId { get; set; }
        public string SearchKeyword { get; set; }
        public long LastRecordId { get; set; }
        public int RecordLimit { get; set; }

        public RequestBusinessSearch_ForSuperAdmin_VM()
        {
            RecordLimit = StaticResources.RecordLimit_Default;
        }

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
            //if (String.IsNullOrEmpty(SearchKeyword)) { sb.Append(Resources.VisitorPanel.SearchValueRequired); vm.Valid = false; }
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }
    }

    public class BusinessSearch_ForSuperAdmin_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int64 BusinessCategoryId { get; set; }
        public Int64 BusinessSubCategoryId { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessLogo { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string BusinessMasterId { get; set; }
        public string BusinessUniqueUserId { get; set; }
        public string BusinessName { get; set; }
        public long UserLoginId { get; set; }
        public int IsCertificateAssigned { get; set; }
        public long CustomFormId { get; set; }
        public string CustomFormName { get; set; }

    }
}