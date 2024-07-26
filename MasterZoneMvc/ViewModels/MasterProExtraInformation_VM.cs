using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MasterProExtraInformation_VM
    {

        public long Id { get; set; }
        public long UserLoginId { get; set; }

        public string Title { get; set; }
        public string ShortDescription { get; set; }

        public long BusinessOwnerLoginId { get; set; }
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
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.BusinessServiceTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ShortDescription)) { sb.Append(Resources.BusinessPanel.BusinessServiceShortDescriptionRequired); vm.Valid = false; }
            if (Mode == 2)
            {

                sb.Append(Resources.BusinessPanel.BusinessServiceStatusRequired);
                vm.Valid = false;

            }




            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

        //public MasterProExtraInformation_VM()
        //    {
        //    Title = string.Empty;
        //    ShortDescription = string.Empty;
        //        Freelance = string.Empty;
        //        Skype = string.Empty;
        //        Languages = string.Empty;


        //}


    }
    public class MasterProExtraInformation_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }

        public string Title { get; set; }
        public string ShortDescription { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
    }
    public class MasterProExtraInformationService_Pagination_SQL_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
    public class MasterProExtraInformationService_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }

        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string BusinessServiceIconWithPath { get; set; }

        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
}