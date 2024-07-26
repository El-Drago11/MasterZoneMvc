using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class BatchViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public long GroupId { get; set; }
        public long InstructorLoginId { get; set; }
        public int StudentMaxStrength { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime ScheduledOnDateTime { get; set; }
        public int ClassDurationSeconds { get; set; }
        public int Status { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class RequestBatchViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long InstructorLoginId { get; set; }
        public string Name { get; set; }
        public int StudentMaxStrength { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime ScheduledOnDateTime { get; set; }
        public int Status { get; set; }
        public int Mode { get; set; }
        public long GroupId { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.ValidInformationMessage;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.BatchNameRequired); vm.Valid = false; }
            else if ((StudentMaxStrength <= 0)) { sb.Append(Resources.BusinessPanel.ErrorMessageLimitOfStudent); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ScheduledStartOnTime_24HF)) { sb.Append(Resources.BusinessPanel.ErrorMessageClassStartTime); vm.Valid = false; }
            else if (String.IsNullOrEmpty(ScheduledEndOnTime_24HF)) { sb.Append(Resources.BusinessPanel.ErrorMessageClassEndTime); vm.Valid = false; }
            else if (GroupId <= 0) { sb.Append(Resources.BusinessPanel.GroupSelection_Required); vm.Valid = false; }
            else if (InstructorLoginId <= 0) { sb.Append(Resources.BusinessPanel.InstructorSelectionRequired); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvaildRequestMessage); vm.Valid = false; }
            else if (DateTime.TryParseExact(ScheduledStartOnTime_24HF, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startTime) &&
               DateTime.TryParseExact(ScheduledEndOnTime_24HF, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endTime))
            {
                if (startTime > endTime)
                {
                    sb.Append(Resources.BusinessPanel.ErrorMessageEndTimeGreaterThen);
                    vm.Valid = false;
                }
            }
            
            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }

    public class Batch_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string Name { get; set; }
        public long GroupId { get; set; }
        public long InstructorLoginId { get; set; }
        public int StudentMaxStrength { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public DateTime ScheduledOnDateTime { get; set; }
        public int ClassDurationSeconds { get; set; }
        public int Status { get; set; }
        public int GroupMembersCount { get; set; }

        public Batch_VM()
        {
            GroupMembersCount = -1; // Not set 
        }
    }

    public class Batch_Pagination_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public int StudentMaxStrength { get; set; }
        public long InstructorLoginId { get; set; }
        public long GroupId { get; set; }
        public int ClassDurationSeconds { get; set; }
        public int Status { get; set; }
        public string GroupName { get; set; }
        public string InstructorFullName { get; set; }
        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        public string InstructorBusinessLogo { get; set; }
        public string InstructorBusinessLogoWithPath { get; set; }
        public string InstructorProfileImage { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string GroupImage { get; set; }
        public string GroupImageWithPath { get; set; }

    }

    public class BatchList_Pagination_SQL_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class Batch_DropdownList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class BatchInfo_VM
    {
        public long InstructorId { get; set; }
        public long UserLoginId { get; set; }
        public string InstructorFullName { get; set; }
        public int InstructorStatus { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string InstructorCategoryName { get; set; }
        public string Designation { get; set; }
        public string BatchDetailJson { get; set; }
        public List<Batches_VM> BatchDetails { get; set; }
    }

    public class Batches_VM
    {
        public long BatchId { get; set; }
        public string BatchName { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public int StudentMaxStrength { get; set; }
        public int StudentCount { get; set; }
        public string StudentImageWithPathJson { get; set; }
        public string[] StudentImageWithPath { get; set; }
        public int ClassDurationSeconds { get; set; }
        public string ClassImageWithPath { get; set; }
        public int Status { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
    }


    //public class BatchDetails_VM
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public string ScheduledStartOnTime_24HF { get; set; }
    //    public string ScheduledEndOnTime_24HF { get; set; }
    //    public int StudentMaxStrength { get; set; }
    //    public long InstructorLoginId { get; set; }
    //    public long GroupId { get; set; }
    //    public int ClassDurationSeconds { get; set; }
    //    public int Status { get; set; }
    //    public string GroupName { get; set; }
    //    public string InstructorFullName { get; set; }
    //    public long SerialNumber { get; set; }
    //    public long TotalRecords { get; set; }
    //    public string InstructorBusinessLogo { get; set; }
    //    public string InstructorBusinessLogoWithPath { get; set; }
    //    public string InstructorProfileImage { get; set; }
    //    public string InstructorProfileImageWithPath { get; set; }
    //    public string GroupImage { get; set; }
    //    public string GroupImageWithPath { get; set; }
    //}
}