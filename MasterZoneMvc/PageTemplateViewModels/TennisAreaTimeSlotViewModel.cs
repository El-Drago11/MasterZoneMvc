using iTextSharp.tool.xml.html.head;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class TennisAreaTimeSlotViewModel
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public long UserLoginId { get; set; }
        public string Time { get; set; }
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
            if (String.IsNullOrEmpty(Time)) { sb.Append(Resources.BusinessPanel.TennisTimeRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
    public class TennisAreaTimeSlot_VM
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public long UserLoginId { get; set; }
        public string Time { get; set; }
        public int Mode { get; set; }

    }
}