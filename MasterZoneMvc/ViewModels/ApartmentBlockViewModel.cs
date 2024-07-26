using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ApartmentBlockViewModel
    {
        public long Id { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class ApartmentBlock_VM
    {
        public long Id { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }
    }

    public class RequestApartmentBlock_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.ApartmentBlockNameRequired); vm.Valid = false; }
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}