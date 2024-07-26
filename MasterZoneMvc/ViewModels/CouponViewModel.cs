using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CouponViewModel
    {

    }

    public class Coupon_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // public string Code { get; set; }
        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public int IsFixedAmount { get; set; } // Fixed = 1 , Percentage = 0

        public decimal DiscountValue { get; set; } // Fixed: 1000.00 , Percentage: 15.00
        public int DiscountFor { get; set; }// All Student = 1, Selected Student = 2

        public string SelectedStudent { get; set; }
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

            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.NameIsRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(StartDate)) { sb.Append(Resources.BusinessPanel.StartDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EndDate)) { sb.Append(Resources.BusinessPanel.EndDateRequired); vm.Valid = false; }
            else if (IsFixedAmount < 0) { sb.Append(Resources.BusinessPanel.FixedAmountRequired); vm.Valid = false; }
            else if (DiscountValue <= 0) { sb.Append(Resources.BusinessPanel.DiscountValueRequired); vm.Valid = false; }
            else if (DiscountFor <= 0) { sb.Append(Resources.BusinessPanel.DiscountForRequired); vm.Valid = false; }
            else if (DiscountFor == 2 && String.IsNullOrEmpty(SelectedStudent)) { sb.Append(Resources.BusinessPanel.SelectedStudentRequired); vm.Valid = false; }
            
            if (vm.Valid == false)
            {
                vm.Message = Resources.BusinessPanel.ErrorMessage + sb.ToString();
            }

            return vm;
        }

    }
    public class Coupon_Pagination_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string StartDate { get; set; }

        public string EndDate { get; set; }
        public int IsFixedAmount { get; set; } // Fixed = 1 , Percentage = 0

        public string AmountTypeName { get; set; }
        public decimal DiscountValue { get; set; } // Fixed: 1000.00 , Percentage: 15.00
                                                   //  public int MaxPerUser { get; set; }
        public int TotalUsed { get; set; }
        public int DiscountFor { get; set; }
        public string DiscountForName { get; set; }

        // All Student = 1, Selected Student = 2
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }

        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
    }

    public class CouponList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class CouponDetailWithStudent_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int DiscountFor { get; set; }
        public string SelectedStudents { get; set; }

        public List<StudentList_ForBusiness_VM> DiscountStudent { get; set; }
    }

    public class CouponDetailForStudent_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string EndDate { get; set; }
        public string BusinessName { get; set; }

    }
}