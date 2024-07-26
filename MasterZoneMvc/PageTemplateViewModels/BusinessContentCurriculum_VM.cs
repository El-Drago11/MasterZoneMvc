using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentCurriculum_VM
    {

        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CurriculumOptions { get; set; }
        public HttpPostedFile CurriculumImage { get; set; }
        public int Mode { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = "Invalid Data!";
            }



            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
             else if (String.IsNullOrEmpty(CurriculumOptions)) { sb.Append(Resources.BusinessPanel.CurriculumOptionsRequired); vm.Valid = false; }
            else if (CurriculumImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(CurriculumImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (CurriculumImage.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append("Image size too large. Please select a smaller image. (upto 5 MB)");
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
    public class BusinessContentCurriculumDetail_VM
    {

        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long UserLoginId { get; set; }
        public string Title { get; set; }
        public string CurriculumOptions { get; set; }
        public string CurriculumImage { get; set; }
        public string CurriculumImageWithPath { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentCurriculumDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class BusinessContentCurriculumDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string CurriculumOptions { get; set; }
        public string CurriculumImage { get; set; }
        public string CurriculumImageWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
}