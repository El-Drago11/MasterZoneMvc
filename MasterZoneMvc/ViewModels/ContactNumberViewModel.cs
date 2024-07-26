using iTextSharp.tool.xml.html.head;
using MasterZoneMvc.Common.ValidationHelpers;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using static QRCoder.PayloadGenerator;

namespace MasterZoneMvc.ViewModels
{
    public class ContactNumberViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Location1 { get; set; }
        public string ContactNumber1 { get; set; }
        public string Location2 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Location3 { get; set; }
        public string ContactNumber3 { get; set; }
        public string Location4 { get; set; }
        public string ContactNumber4 { get; set; }
        public string Location5 { get; set; }
        public string ContactNumber5 { get; set; }
        public string Location6 { get; set; }
        public string ContactNumber6 { get; set; }
        public string Location7 { get; set; }
        public string ContactNumber7 { get; set; }
        public string Location8 { get; set; }
        public string ContactNumber8 { get; set; }
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

            if (Location1 != "")
            {
                if (String.IsNullOrEmpty(ContactNumber1))
                {
                    sb.Append(Resources.SuperAdminPanel.ContactNumber1Required);
                    vm.Valid = false;
                }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber1))
                {
                    sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired);
                    vm.Valid = false;
                }
            }

            if (Location2 != "")
            {
                if (String.IsNullOrEmpty(Location2)) { sb.Append(Resources.SuperAdminPanel.Location2Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber2)) { sb.Append(Resources.SuperAdminPanel.ContactNumber2Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber1)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }

            }

            if(Location3 != "")
            {
                if (String.IsNullOrEmpty(Location3)) { sb.Append(Resources.SuperAdminPanel.Location3Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber3)) { sb.Append(Resources.SuperAdminPanel.ContactNumber3Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber3)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            }

            if(Location4 != "")
            {
                if (String.IsNullOrEmpty(Location4)) { sb.Append(Resources.SuperAdminPanel.Location4Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber4)) { sb.Append(Resources.SuperAdminPanel.ContactNumber4Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber4)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            }

            if(Location5  != "")
            {
                if (String.IsNullOrEmpty(Location5)) { sb.Append(Resources.SuperAdminPanel.Location5Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber5)) { sb.Append(Resources.SuperAdminPanel.ContactNumber5Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber5)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            }

            if(Location6 != "")
            {
                if (String.IsNullOrEmpty(Location6)) { sb.Append(Resources.SuperAdminPanel.Location6Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber6)) { sb.Append(Resources.SuperAdminPanel.ContactNumber6Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber6)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            }
            if(Location7 != "")
            {
                if (String.IsNullOrEmpty(Location7)) { sb.Append(Resources.SuperAdminPanel.Location7Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber7)) { sb.Append(Resources.SuperAdminPanel.ContactNumber7Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber7)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            }

            if(Location8 !="")
            {
                if (String.IsNullOrEmpty(Location8)) { sb.Append(Resources.SuperAdminPanel.Location8Required); vm.Valid = false; }
                else if (String.IsNullOrEmpty(ContactNumber8)) { sb.Append(Resources.SuperAdminPanel.ContactNumber8Required); vm.Valid = false; }
                else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber8)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            }


            //if (String.IsNullOrEmpty(Location1)) { sb.Append(Resources.SuperAdminPanel.Location1Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber1)) { sb.Append(Resources.SuperAdminPanel.ContactNumber1Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber1)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location2)) { sb.Append(Resources.SuperAdminPanel.Location2Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber2)) { sb.Append(Resources.SuperAdminPanel.ContactNumber2Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber1)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location3)) { sb.Append(Resources.SuperAdminPanel.Location3Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber3)) { sb.Append(Resources.SuperAdminPanel.ContactNumber3Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber3)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location4)) { sb.Append(Resources.SuperAdminPanel.Location4Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber4)) { sb.Append(Resources.SuperAdminPanel.ContactNumber4Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber4)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location5)) { sb.Append(Resources.SuperAdminPanel.Location5Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber5)) { sb.Append(Resources.SuperAdminPanel.ContactNumber5Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber5)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location6)) { sb.Append(Resources.SuperAdminPanel.Location6Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber6)) { sb.Append(Resources.SuperAdminPanel.ContactNumber6Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber6)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location7)) { sb.Append(Resources.SuperAdminPanel.Location7Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber7)) { sb.Append(Resources.SuperAdminPanel.ContactNumber7Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber7)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Location8)) { sb.Append(Resources.SuperAdminPanel.Location8Required); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(ContactNumber8)) { sb.Append(Resources.SuperAdminPanel.ContactNumber8Required); vm.Valid = false; }
            //else if (!PhoneNumberValidationHelper.IsValidPhoneNumber(ContactNumber8)) { sb.Append(Resources.VisitorPanel.ValidPhoneNumberRequired); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class ContactNumberDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Location1 { get; set; }
        public string ContactNumber1 { get; set; }
        public string Location2 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Location3 { get; set; }
        public string ContactNumber3 { get; set; }
        public string Location4 { get; set; }
        public string ContactNumber4 { get; set; }
        public string Location5 { get; set; }
        public string ContactNumber5 { get; set; }
        public string Location6 { get; set; }
        public string ContactNumber6 { get; set; }
        public string Location7 { get; set; }
        public string ContactNumber7 { get; set; }
        public string Location8 { get; set; }
        public string ContactNumber8 { get; set; }
        public int Mode { get; set; }
    }
}