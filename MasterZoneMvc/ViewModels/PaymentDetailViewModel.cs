using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class PaymentDetailViewModel
    {
        public Int64 Id { get; set; }
        public string PaymentModeDetail { get; set; }
    }

    public class Card_VM
    {
        public long PaymentDetailId { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }

        public Error_VM ValidInformation()
        {
            var utcDateTime = DateTime.UtcNow;
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(CardNumber)) { sb.Append(Resources.VisitorPanel.CardNumberRequired); vm.Valid = false; }
            else if (!String.IsNullOrEmpty(CardNumber) && CardNumber.Length != 16) { sb.Append(Resources.ErrorMessage.CardNumberMustBeOf16Digit); vm.Valid = false; }
            else if (String.IsNullOrEmpty(CardName)) { sb.Append(Resources.VisitorPanel.CardNameRequired); vm.Valid = false; }
            else if (ExpMonth <= 0) { sb.Append(Resources.VisitorPanel.ExpiryMonthRequired); vm.Valid = false; }
            else if (ExpYear <= 0) { sb.Append(Resources.VisitorPanel.ExpiryYearRequired); vm.Valid = false; }
            else if (ExpYear < utcDateTime.Year) { sb.Append(Resources.VisitorPanel.ValidExpiryYearRequired); vm.Valid = false; }
            else if (ExpMonth < utcDateTime.Month && ExpYear < utcDateTime.Year) { sb.Append(Resources.VisitorPanel.ValidExpiryMonthRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }



    }

    public class Paytm_VM
    {
        public long PaymentDetailId { get; set; }
        public string PaytmId { get; set; }
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
            if (String.IsNullOrEmpty(PaytmId)) { sb.Append(Resources.VisitorPanel.PaytmIdRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            return vm;
        }

    }

    public class UPI_VM
    {
        public long PaymentDetailId { get; set; }
        public string UPIId { get; set; }
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
            if (String.IsNullOrEmpty(UPIId)) { sb.Append(Resources.VisitorPanel.UPIIdRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }
}