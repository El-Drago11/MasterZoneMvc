using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentPlan_PPCMetaViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessPlanTitle { get; set; }
        public string BusinessPlanDescription { get; set; }
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
            if (String.IsNullOrEmpty(BusinessPlanTitle)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(BusinessPlanDescription)) { sb.Append(Resources.BusinessPanel.VideoDescriptionRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
    public class BusinessContentPlan_PPCMetaDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string BusinessPlanTitle { get; set; }
        public string BusinessPlanDescription { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long BusinessPlanDurationTypeId { get; set; }
        public string BusinessPlanDurationTypeName { get; set; }
        public string Description { get; set; }
        public decimal CompareAtPrice { get; set; }
        public int Status { get; set; }
        public string PlanImage { get; set; }
        public string PlanImageWithPath { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}