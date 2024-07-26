using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using static QRCoder.PayloadGenerator.SwissQrCode;
using System.Windows;

namespace MasterZoneMvc.ViewModels
{
    public class UserExperienceDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string EndMonth { get; set; }
        public string EndYear { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CompanyName)) { sb.Append(Resources.BusinessPanel.CompanyNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartMonth)) { sb.Append(Resources.BusinessPanel.StartMonthRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndMonth)) { sb.Append(Resources.BusinessPanel.EndMonthRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartYear)) { sb.Append(Resources.BusinessPanel.StartYearRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndYear)) { sb.Append(Resources.BusinessPanel.EndYearRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartDate)) { sb.Append(Resources.BusinessPanel.StartDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndDate)) { sb.Append(Resources.BusinessPanel.EndDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
           

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class Experience_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class UserExperienceDetail_ByPagination
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string EndMonth { get; set; }
        public string EndYear { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }


    public class UserExperienceDetail_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string EndMonth { get; set; }
        public string EndYear { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }
    }
}