using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class EventCategoryViewModel
    {

        public long Id { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class EventCategory_VM
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public int IsDeleted { get; set; }

    }
    public class RequestEventCategory_VM
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }

        public int Status { get; set; }
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
            if (String.IsNullOrEmpty(CategoryName)) { sb.Append(Resources.SuperAdminPanel.CategoryNameRequired); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }
}