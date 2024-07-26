using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessPlanViewModel
    {
        public long Id { get; set; }

        public long BusinessOwnerLoginId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        public long BusinessPlanDurationTypeId { get; set; }

        public string Description { get; set; }

        public decimal CompareAtPrice { get; set; }

        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }

        public string PlanImage { get; set; }
        public decimal DiscountPercent { get; set; }
    }

    public class RequestBusinessPlan_VM {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long BusinessPlanDurationTypeId { get; set; }
        public string Description { get; set; }
        public decimal CompareAtPrice { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public int PlanType { get; set; }
        public int PlanTypeTitle { get; set; }
        public HttpPostedFile PlanImage { get; set; }
        public decimal DiscountPercent { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.PlanNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if (Price <= 0) { sb.Append(Resources.BusinessPanel.PricePlanMustBeGreaterThanZero_ErrorMessage); vm.Valid = false; }
            else if (CompareAtPrice != 0 && CompareAtPrice <= Price) { sb.Append(Resources.BusinessPanel.CompareAtPriceMustGreaterThanPrice_ErrorMessage); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.BusinessPanel.InvalidStatusRequired); vm.Valid = false; }
            else if (BusinessPlanDurationTypeId < 0) { sb.Append(Resources.BusinessPanel.InvalidDurationType_ErrorMessage); vm.Valid = false; }
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
                else if (PlanImage.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.SuperAdminPanel.ValidFileSize_ErrorMessage, "10 MB"));
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

    public class BusinessPlan_VM
    {
        public long Id { get; set; }
        //public long BusinessOwnerId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long BusinessPlanDurationTypeId { get; set; }
        public string BusinessPlanDurationTypeName { get; set; }
        public string Description { get; set; }
        public decimal CompareAtPrice { get; set; }
        public int Status { get; set; }
        public int PlanType { get; set; }
        public string PlanImage { get; set; }
        public string PlanDurationTypeKey { get; set; }
        public string PlanImageWithPath { get; set; }
        public decimal DiscountPercent { get; set; }
    }

    public class CoachesInstructorPackageDetail
    {
        public long Id { get; set; }
        public string PlanImage { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
    public class UserPlanDetail_VM
    {

        public long Id { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PlanDurationTypeName { get; set; }
        public string PlanImage { get; set; }
        public string ProfileImageWithPath { get; set; }
    }
}