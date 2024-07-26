using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class GroupViewModel
    {
    
    }

    public class Group_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }
        public string GroupImage { get; set; }
        public string GroupImageWithPath { get; set; }

        public List<GroupMember_ForBusiness_VM> GroupStudents { get; set; }
    }

    public class RequestGroupViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }
        public HttpPostedFile ProfileImage { get; set; }
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
                return vm;
            }

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.GroupNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.GroupDescriptionRequired); vm.Valid = false; }
            else if (ProfileImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ProfileImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (ProfileImage.ContentLength > 2 * 1024 * 1024) // 2 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "2 MB"));
                }

                vm.Valid = isValidImage;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class GroupList_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class Group_Pagination_VM {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupType { get; set; }
        public string GroupImage { get; set; }
        public string GroupImageWithPath { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }

        public long SerialNumber { get; set; }
        public long TotalRecords { get; set; }
        
        public string EncryptedUserLoginId { get; set; }

        public int CountUnreadMessage { get; set; }
    }

    public class RequestGroupMember_VM
    {
        public long GroupId { get; set; }
        public long MemberLoginId { get; set; }
    }

    public class GroupNameList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class UserClassGroupList_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupImage { get; set; }
        public string GroupImageWithPath { get; set; }
        public long BusinessOwnerId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public string ClassName { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string ScheduledEndOnTime_24HF { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public string InstructorName { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public int InstructorIsCertified { get; set; }
        public string InstructorUniqueUserId { get; set; }

        public long BatchId { get; set; }
        public string BatchName { get; set; }
        public long ClassCategoryTypeId { get; set; }
        public string ClassCategoryTypeName { get; set; }

    }

    public class UserClassGroupBusinessList_VM
    {
        public long BusinessOwnerId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLogoWithPath { get; set; }
        public int TotalGroupCount { get; set; }
        public List<UserClassGroupList_VM> UserClassGroupList { get; set; }
    }

    public class AddGroupMember_VM
    {
        public long GroupId { get; set; }
        public long GroupUserLoginId { get; set; }
        public long MemberLoginId { get; set; }
    }

    public class GroupMember_ForBusiness_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber_CountryCode { get; set; }
        public string MasterId { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string StaffCategoryName { get; set; }


    }
}