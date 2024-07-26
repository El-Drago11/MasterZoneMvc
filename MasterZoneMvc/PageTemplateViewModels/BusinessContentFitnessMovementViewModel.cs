using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentFitnessMovementViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Requirements { get; set; }
        public string Investment { get; set; }
        public string Inclusions { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = "Invalid Data!";
            }



            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Requirements)) { sb.Append("Requirement Is Required"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Investment)) { sb.Append("Investment Is Required"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Inclusions)) { sb.Append("Inclusions Is Required"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
           
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class BusinessContentFitnessMovementDetailViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Requirements { get; set; }
        public string Investment { get; set; }
        public string Inclusions { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentFitnessMovementDetail_Pagination_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Requirements { get; set; }
        public string Investment { get; set; }
        public string Inclusions { get; set; }
        public string Description { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BusinessContentFitnessMovementDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}