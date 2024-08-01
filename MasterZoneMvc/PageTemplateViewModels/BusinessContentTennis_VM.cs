using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentTennis_VM
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public HttpPostedFile TennisImage { get; set; }
        public decimal Price { get; set; }
        public decimal OtherPrice { get; set; }
        public decimal CommercialPrice { get; set; }
        public decimal BasicPrice { get; set; }
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(SubTitle)) { sb.Append(Resources.BusinessPanel.SubTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if ( BasicPrice <= 0)
            {
                sb.Append(Resources.BusinessPanel.PriceRequired); vm.Valid = false;
            }
            else if ( CommercialPrice <= 0)
            {
                sb.Append(Resources.BusinessPanel.PriceRequired); vm.Valid = false;
            }else if ( OtherPrice <= 0)
            {
                sb.Append(Resources.BusinessPanel.PriceRequired); vm.Valid = false;
            }
         
            else if (TennisImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(TennisImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (TennisImage.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidImage;
            }
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
    public class BusinessContentTennisDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string TennisImage { get; set; }
        public string TennisImageWithPath { get; set; }
        public decimal Price { get; set; }
        public decimal OtherPrice { get; set; }
        public decimal CommercialPrice { get; set; }
        public decimal BasicPrice { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class TennisDetail_Pagination_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal BasicPrice { get; set; }
        public decimal CommercialPrice { get; set; }
        public decimal OtherPrice { get; set; }

        public string TennisImage { get; set; }
        public string TennisImageWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class TennisDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class TennisBokingDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FullName { get; set; }
        public string RoomTime { get; set; }
        public string RoomService { get; set; }
        public string SelectDate { get; set; }
        public int PlayerCount { get; set; }
        public string TennisTitle { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string TennisImage { get; set; }
        public string TennisImageWithPath { get; set; }
        public decimal Price { get; set; }
        public int Request { get; set; }

    }
}