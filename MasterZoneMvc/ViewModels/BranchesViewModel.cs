using MasterZoneMvc.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BranchesViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long BranchBusinessLoginId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.NameIsRequired); vm.Valid = false; }
           

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }


    public class BranchDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long BranchBusinessLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string BranchName { get; set; }
        public string BusinessOwnerName { get; set; }
        public string BusinessName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BusinessLogo { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BranchDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }


    public class BranchDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long BranchBusinessLoginId { get; set; }
        public long BusinessSubCategoryId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email     { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
    }
}