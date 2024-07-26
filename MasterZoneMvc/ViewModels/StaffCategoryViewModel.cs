using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StaffCategoryViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class StaffCategory_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
        public int Mode { get; set; }
        public int IsDeleted { get; set; }

    }
    public class RequestStaffCategory_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public int IsActive { get; set; }
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


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }


    }
}