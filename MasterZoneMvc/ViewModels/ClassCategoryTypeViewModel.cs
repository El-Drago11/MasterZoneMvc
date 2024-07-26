using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ClassCategoryTypeViewModel
    {
        public long Id { get; set; }
        public long BusinessCategoryId { get; set; }
        public long ParentClassCategoryTypeId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public int ShowOnHomePage { get; set; }
        public string Description { get; set; }
    }

    public class RequestClassCategoryType
    {
        public long Id { get; set; }
        public long BusinessCategoryId { get; set; }
        public long ParentClassCategoryTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int Mode { get; set; }
        public HttpPostedFile Image { get; set; }
        public long BusinessOwnerLoginId { get; set; }

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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.SuperAdminPanel.ClassCategoryType_NameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.SuperAdminPanel.DescriptionRequired); vm.Valid = false; }
            //else if (ParentClassCategoryTypeId <= 0) { sb.Append(Resources.SuperAdminPanel.ParentBusinessCategoryRequired); vm.Valid = false; }
            //else if (BusinessCategoryId <= 0) { sb.Append(Resources.SuperAdminPanel.ClassCategoryType_BusinessCategoryRequired); vm.Valid = false; }
            else if (Mode <= 0) { sb.Append(Resources.ErrorMessage.InvaildRequestMessage); vm.Valid = false; }
            else if (Id <= 0 && Mode == 2) { sb.Append(Resources.ErrorMessage.InvaildRequestMessage); vm.Valid = false; }
            else if (Image != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Image.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (Image.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
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

    public class ClassCategoryType_VM
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int IsActive { get; set; }

        public string ImageWithPath { get; set; }
        public long ParentClassCategoryTypeId { get; set; }
        public string ParentClassCategoryTypeName { get; set; }
        public int ShowOnHomePage { get; set; }
        public string Description { get; set; }
        public List<ClassCategoryType_VM> SubClassCategory { get; set; }

    }
}