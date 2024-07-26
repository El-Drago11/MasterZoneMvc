using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ApartmentViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string Blocks { get; set; }
        public string Areas { get; set; }

        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class Apartment_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string Blocks { get; set; }
        public string Areas { get; set; }

        public int Status { get; set; }
    }

    public class Apartment_Dropdown_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class RequestApartmentViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.ApartmentNameRequired); vm.Valid = false; }
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class Apartment_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class ApartmentPagination_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public string Blocks { get; set; }
        public string Areas { get; set; }
        public int Status { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }


}