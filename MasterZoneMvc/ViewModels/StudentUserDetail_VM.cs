using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StudentUserDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public string MasterId { get; set; }
        public int IsBlocked { get; set; }
        public string BlockReason { get; set; }
        public string BusinessStudentProfileImage { get; set; }
    }
    public class StudentUser_Pagination_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
        public string Email { get; set; }
        public int IsBlocked { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_FormatDate { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
        public string BlockReason { get; set; }

    }

    public class StudentUser_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class StudentCourseDetail_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string ScheduledStartOnTime_24HF { get; set; }
        public string InstructorName { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImageWithPath { get; set; }
    }
    public class UserImageDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get;set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }
    }
    
}