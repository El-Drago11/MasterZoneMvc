using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class HomePageClassCategorySectionViewModel
    {
        public long Id { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
    }
    
    public class HomePageClassCategorySection_VM
    {
        public long Id { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public int Status { get; set; }
    }

    public class RequestHomePageClassCategorySection_VM
    {
        public long Id { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
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
            if (ClassCategoryTypeId <= 0) { sb.Append(Resources.SuperAdminPanel.ClassCategoryTypeRequired); vm.Valid = false; }
            else if (Status < 0 || Status > 1) { sb.Append(Resources.SuperAdminPanel.InvalidActiveStatusRequired); vm.Valid = false; }
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}