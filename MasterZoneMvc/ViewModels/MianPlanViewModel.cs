using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MianPlanViewModel
    {
    }

    public class RequestMainPlan_VM
    {
        public long Id { get; set; }

        //[ForeignKey(nameof(BusinessOwner))]
        public long UserLoginId { get; set; }

        public string Name { get; set; }
        public string PlanDurationTypeKey { get; set; }
        public decimal Price { get; set; }
        public decimal CompareAtPrice { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public HttpPostedFile PlanImage { get; set; }
        public int Status { get; set; }
        public string PlanPermission { get; set; }
        public int Mode { get; set; }
        public int planType { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.SuperAdminPanel.PlanNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.SuperAdminPanel.PlanDescriptionRequired); vm.Valid = false; }
            else if (Price <= 0) { sb.Append(Resources.SuperAdminPanel.PlanPriceValueRequired); vm.Valid = false; }
            else if (CompareAtPrice != 0 && CompareAtPrice <= Price) { sb.Append(Resources.SuperAdminPanel.CompareAtPriceMustGreaterThanPrice_ErrorMessage); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.AppendLine(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PlanDurationTypeKey)) { sb.Append(Resources.SuperAdminPanel.PlanDurationTypeRequired); vm.Valid = false; }

            else if (PlanImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(PlanImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.SuperAdminPanel.ValidImageFile_ErrorMessage);
                }
                else if (PlanImage.ContentLength > 5 * 1024 * 1024) // 5 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.SuperAdminPanel.ValidFileSize_ErrorMessage, "5 MB"));
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

    public class MainPlan_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PlanDurationTypeKey { get; set; }
        public string PlanDurationTypeKeyName { get; set; }
        public decimal Price { get; set; }
        public string PlanImage { get; set; }
        public string PlanImageWithPath { get; set; }
        public decimal CompareAtPrice { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string PlanPermission { get; set; }
        public string MasterId { get; set; }
        public int ShowOnHomePage { get; set; }
        public int PlanType { get; set; }

        public List<PermissionListMainPaln_VM> MainPlanPermissionsList { get; set; }
    }
    public class PermissionListMainPaln_VM : MainPlan_VM
    {
        public string MainPlanPermissions { get; set; }
    }
}