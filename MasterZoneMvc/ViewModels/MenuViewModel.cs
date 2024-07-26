using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MenuViewModel
    {
        public long Id { get; set; }
        public long ParentMenuId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string PageLink { get; set; }
        public int IsActive { get; set; }
        public string Tag { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class VisitorMenu_VM {
        public long Id { get; set; }
        public long ParentMenuId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string PageLink { get; set; }
        public int IsActive { get; set; }
        public int IsShowOnHomePage { get; set; }
        public string Tag { get; set; }
        public string MenuImageWithPath { get; set; }
        public int SortOrder { get; set; }
    }


    public class RequestMenuViewModel
    {
        public long Id { get; set; }
        public long ParentMenuId { get; set; }

        public string Name { get; set; }
        public string PageLink { get; set; }

        public int IsActive { get; set; }
        public int IsShowOnHomePage { get; set; }

        public string MenuTag { get; set; }
        public string Image { get; set; }
        public long SubmittedByLoginId { get; set; }
        public int Mode { get; set; }
        public int SortOrder { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.SuperAdminPanel.MenuNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(MenuTag)) { sb.Append(Resources.SuperAdminPanel.MenuTagRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(PageLink)) { sb.Append(Resources.SuperAdminPanel.MenuPageLinkRequired); vm.Valid = false; }
            else if (SortOrder < 0) { sb.Append(Resources.SuperAdminPanel.MenuSortOrderRequired); vm.Valid = false; }
            else if (IsActive < 0 || IsActive > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}