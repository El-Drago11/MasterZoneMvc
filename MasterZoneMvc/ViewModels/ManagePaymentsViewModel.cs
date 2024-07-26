using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ManagePaymentsViewModel
    {
    }

    public class Manage_Payment_Pagination_VM
    {
        public string TransactionID { get; set; }
        public string Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ItemType { get; set; }
        public decimal TotalDiscount { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessMasterId { get; set; }
        public long UserLoginId { get; set; }
        public string UserMasterId { get; set; }
        public string UserImage { get; set; }
        public long UserRoleId { get; set; }

    }
    public class PaymentsList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 OwnerUserLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class Manage_BranchStudentPayment_Pagination_VM
    {
        public long CreatedByLoginId { get; set; }
        public string TransactionID { get; set; }
        public string MasterId { get; set; }
        public string Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ItemType { get; set; }
        public decimal TotalDiscount { get; set; }
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }

    }

    public class BranchPaymentsList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}