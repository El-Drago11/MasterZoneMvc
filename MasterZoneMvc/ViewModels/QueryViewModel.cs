using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class QueryViewModel
    {
        public long Id { get; set; }

        public long StudentId { get; set; }

        public long BusinessOwnerId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public int IsReplied { get; set; }
        public string ReplyBody { get; set; }
        public DateTime RepliedOn { get; set; }

        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }


    public class Query_Pagination_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long StudentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int IsReplied { get; set; }
        public string ReplyBode { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime RepliedOn { get; set; }
        //public long CreatedByLoginId { get; set; }
        //public DateTime UpdatedOn { get; set; }
        //public long UpdatedByLoginId { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public string RepliedOn_FormatDate { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentName { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public long StudentUserLoginId { get; set; }
        public string StudentProfileImageWithPath { get; set; }


    }

    public class Queries_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long BusinessOwnerId { get; set; }
        public string ReplyBody { get; set; }
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
            if (BusinessOwnerId <= 0) { sb.Append(Resources.VisitorPanel.SelectBussinessOwnerRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.VisitorPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.VisitorPanel.DescriptionRequired); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvalidMode); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }

        public Error_VM ValidReplyInformation()
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
            if (Id <= 0) { sb.Append("Select Business Owner!"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ReplyBody)) { sb.Append(Resources.BusinessPanel.QueryReplyMessageRequired); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvalidMode); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }

    }

    public class QueryDetail_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public int IsReplied { get; set; }
        public string BusinessName { get; set; }
        public string RepliedOn_FormatDate { get; set; }
        public string ReplyBody { get; set; }
        public string Description { get; set; }
    }

    public class QueryDescription_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int IsReplied { get; set; }
        public string RepliedOn_FormatDate { get; set; }
        public string ReplyBody { get; set; }
        public long BusinessOwnerId { get; set; }
        public string BusinessName { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public string BusinessMasterId { get; set; }
    }

    public class ManageQuery_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}