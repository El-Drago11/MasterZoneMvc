using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class GroupMessageViewModel
    {
    }

    public class GroupMessageChat_VM
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
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

    public class GroupDetailForMessages_VM
    {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerId { get; set; }
        public long SenderUserloginId { get; set; }
        public long ReceiverUserLoginId { get; set; }
        public string EncryptedUserLoginId { get; set; }
        public string GroupProfileImageWithPath { get; set; }
        public string StudentProfileImageWithPath { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string FullName { get; set; }
        public string Messagebody { get; set; }
        public string TimeFormatted { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string Time { get; set; }
        public long CreatedByLoginId { get; set; }
     
        public string GroupName { get; set; }
        public string BusinessName { get; set; }
        public string batchStartTime { get; set; }
        public string batchEndTime { get; set; }
        public int SenderStatus { get; set; }
        public int SenderStatusCount { get; set; }
        public int NewMessageCount { get; set; }
    }


    public class GroupMessage_VM
    {
        public long Id { get; set; }
        public string GroupProfileImageWithPath { get; set; }
        public string GroupName { get; set; }
        public long UserLoginId { get; set; }
        public string batchStartTime { get; set; }
        public string batchEndTime { get; set; }
        public int? CountStudent { get; set; }
        public int? TotalCountSeat { get; set; }
        public string BusinessName { get; set; }
        public string CreatedByMasterId { get; set; }
        public string InstructorMasterId { get; set; }
        public string InstructorName { get; set; }
        public string CreatedOn { get; set; }

    }
}