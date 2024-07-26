using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentNoticeBoard_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string StartDate { get; set; }
        public string Description { get; set; }
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
            if (String.IsNullOrEmpty(StartDate)) { sb.Append(Resources.BusinessPanel.StartDateRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }




            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }


    public class BusinessContentNoticeBoardDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string StartDate { get; set; }
        public string Description { get; set; }
        public int StartDay { get; set; }
        public string StartMonth { get; set; }
        public int StartYear { get; set; }
        public int Mode { get; set; }

    }
}