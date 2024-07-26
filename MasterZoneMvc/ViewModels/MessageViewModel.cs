using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class MessageViewModel
    {
    }

    public class MessageChat_VM
    {
        public long Id { get; set; }
        public long SenderUserloginId { get; set; }
        public long ReceiverUserLoginId { get; set; }
        public string Messagebody { get; set; }
        public int SenderStatus { get; set; }
        public int ReceiverStatus { get; set; }
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
            if (String.IsNullOrEmpty(Messagebody)) { sb.Append(Resources.BusinessPanel.MessageTextRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }

    public class StudentMessage_VM
    {
        public long Id { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long UserLoginId { get; set; }

    }
    public class Message_VM
    {
        public long Id { get; set; }
        public string Messagebody { get; set; }
        public string Time { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long SenderUserLoginId { get; set; }
        public long ReceiverUserLoginId { get; set; }
    }


    /// <summary>
    /// ///View
    /// </summary>
    public class StudentDetail
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string StudentName { get; set; }
        public int ReceiverStatus { get; set; }
        public int ReceiverStatusCount { get; set; }
        public int NewMessageCount { get; set; }
    }

    public class RequestMessageReadUnread_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int ReceiverStatus { get; set; }
        public int ReceiverStatusCount { get; set; }
        public int SenderStatusCount { get; set; }
        public int SenderStatuCount { get; set; }
    }


    public class Business_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string BusinessName { get; set; }
        public int SenderStatus { get; set; }
        public int SenderStatusCount { get; set; }
        public int NewMessageCount { get; set; }
    }

    public class BusinessDetailForMessages_VM {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string EncryptedUserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public int IsVerified { get; set; }
        public string BusinessName { get; set; }
        public int SenderStatus { get; set; }
        public int SenderStatusCount { get; set; }
        public int NewMessageCount { get; set; }
    }

    public class StudentDetailForMessage_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string EncryptedUserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string StudentName { get; set; }
        public int ReceiverStatus { get; set; }
        public int ReceiverStatusCount { get; set; }
        public int NewMessageCount { get; set; }
        public string LastMessageDateTime { get; set; }
        public string MasterId { get; set; }
    }
}