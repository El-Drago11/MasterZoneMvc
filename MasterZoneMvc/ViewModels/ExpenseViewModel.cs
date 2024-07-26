using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ExpenseViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; } //Staff-UserLoginId       
        public decimal ExpenseAmount { get; set; }
        public string ExpenseDescription { get; set; }
        public string ExpenseDate { get; set; }
        public DateTime ExpenseDate_DateTimeFormat { get; set; }

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

            if (ExpenseAmount < 0) { sb.Append(Resources.BusinessPanel.Expense_AmountRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ExpenseDescription)) { sb.Append(Resources.BusinessPanel.Expense_DescriptionRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ExpenseDate)) { sb.Append(Resources.BusinessPanel.Expense_DateRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
        public class ExpenseList_VM
        {
            public long Id { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string ExpenseDate { get; set; }
        }
        public class BusinessExpenseList_VM
        {
            public long Id { get; set; }
            public string StaffName { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public string Expense_DateTimeFormat { get; set; }
        }


        public class Expense_Pagination_VM
        {
            public long Id { get; set; }
            //public string StaffName { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime ExpenseDate_DateTimeFormat { get; set; }
            public string Expense_DateTimeFormat { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
            public long TotalRecords { get; set; }
            public long SerialNumber { get; set; }

        }
        public class Expense_Pagination_SQL_Params_VM
        {
            public Int64 Id { get; set; }
            public Int64 LoginId { get; set; }
            public Int64 BusinessAdminLoginId { get; set; }
            public int Mode { get; set; }
            public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
        }

        public class BusinessExpense_Pagination_VM
        {
            public long Id { get; set; }
            public string StaffName { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime ExpenseDate_DateTimeFormat { get; set; }
            public string Expense_DateTimeFormat { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
            public long TotalRecords { get; set; }
            public long SerialNumber { get; set; }

        }


        public class BusinessExpense_Pagination_SQL_Params_VM
        {
            public Int64 Id { get; set; }
            public Int64 LoginId { get; set; }
            public Int64 BusinessAdminLoginId { get; set; }
            public int Mode { get; set; }
            public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
        }
        public class BusinessExpense_ViewModel
        {
            public long Id { get; set; }
            public long UserLoginId { get; set; } //Staff-UserLoginId       
            public int Status { get; set; }
            public string Remarks { get; set; }

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

                if (Status < 0) { sb.Append(Resources.BusinessPanel.Expense_StatusRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(Remarks)) { sb.Append(Resources.BusinessPanel.Expense_RemarkDescriptionRequired); vm.Valid = false; }

                if (vm.Valid == false)
                {
                    vm.Message = sb.ToString();
                }

                return vm;
            }
        }

    }
}