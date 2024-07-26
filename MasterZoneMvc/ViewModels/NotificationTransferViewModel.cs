using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class NotificationTransferViewModel
    {
        public long Id { get; set; }
        public string TransferSenderId { get; set; }
        public string MessageNotification { get; set; }

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
            if (String.IsNullOrEmpty(TransferSenderId))
            {
                sb.Append(Resources.BusinessPanel.SelectBusinessOwnerRequired); vm.Valid = false;
            }
            else if (String.IsNullOrEmpty(MessageNotification)) { sb.Append(Resources.BusinessPanel.SelectNotificationDescriptionRequired); vm.Valid = false; }
                
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}