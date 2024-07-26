using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ReviewViewModel
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public int Rating { get; set; }
        public string ReviewBody { get; set; }
        public long ReviewerUserLoginId { get; set; }

        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class RequestReview_VM
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public decimal Rating { get; set; }
        public string ReviewBody { get; set; }
        public long ReviewerUserLoginId { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(ReviewBody)) { sb.Append(Resources.BusinessPanel.ReviewBodyRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class ReviewPagintation_VM
    {
        public long Id { get; set; }
        public string StudentName { get; set; }
        public string Name { get; set; }
        public long ItemId { get; set; }
        public int Rating { get; set; }
        public string ReviewBody { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }

        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class Review_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }


    public class Review_VM
    {
        public long Id { get; set; }
        public string StudentName { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public string ReviewBody { get; set; }
        public string CreatedOn_FormatDate { get; set; }

    }

    public class BusinessRatingReview_VM
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string ItemType { get; set; }
        public int Rating { get; set; }
        public string ReviewBody { get; set; }
        public long ReviewerUserLoginId { get; set; }
        public int Mode { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(ReviewBody)) { sb.Append(Resources.VisitorPanel.RatingDescriptionRequired); vm.Valid = false; }
            else if (Rating <= 0) { sb.Append(Resources.VisitorPanel.RatingReviewRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}