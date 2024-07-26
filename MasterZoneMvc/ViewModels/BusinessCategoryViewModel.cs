using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BusinessCategoryViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int IsActive { get; set; }
        public long ParentBusinessCategoryId { get; set; }
        public string CategoryKey { get; set; }

        public int ProfilePageTypeId { get; set; }

        public string MenuTag { get; set; }
    }

    public class BusinessCategory_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
        public long ParentBusinessCategoryId { get; set; }
        public string CategoryImage { get; set; }
        public string CategoryImageWithPath { get; set; }
        public string CategoryKey { get; set; }

        public int ProfilePageTypeId { get; set; }

        public string MenuTag { get; set; }
    }

    public class BusinessCategoryHierarchy_VM : BusinessCategory_VM
    {
        public List<BusinessCategory_VM> SubCategories { get; set; }
    }

    public class RequestBusinessCategoryViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int IsActive { get; set; }
        public long ParentBusinessCategoryId { get; set; }
        public string CategoryKey { get; set; }

        public int ProfilePageTypeId { get; set; }

        public string MenuTag { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.SuperAdminPanel.CategoryNameRequired); vm.Valid = false; }
            else if (IsActive < 0 || IsActive > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            else if (ParentBusinessCategoryId < 0) { sb.Append(Resources.SuperAdminPanel.InvalidParentCategoryRequired); vm.Valid = false; }
            else if (ParentBusinessCategoryId > 0 && ProfilePageTypeId < 0) { sb.Append(Resources.SuperAdminPanel.ProfilePageTypeRequired_ErrorMessage); vm.Valid = false; }
            else if (ParentBusinessCategoryId > 0 && String.IsNullOrEmpty(CategoryKey)) { sb.Append(Resources.SuperAdminPanel.CategoryKeyRequired_ErrorMessage); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

}