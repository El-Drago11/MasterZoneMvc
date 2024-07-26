using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Models
{
    public class BusinessCourseCategory
    {
        [Key]
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseCategoryImage { get; set; }
        public int Status { get; set; }

        // Created & updated
        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }



    public class BusinessContentCourseCategoryDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseCategoryImage { get; set; }
        public string CourseCategoryImageWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BusinessContentCourseCategoryDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}