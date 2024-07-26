using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ClassFeatureViewModel
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public DateTime CreatedOn { get; set; }
        public Int64 CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedByLoginId { get; set; }
    }

    public class RequestClassFeature_Params
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Mode { get; set; }

        public HttpPostedFile ClassFeatureIcon { get; set; }

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
            if (ClassId <= 0) { sb.Append(Resources.BusinessPanel.ClassFeature_ClassIdRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.ClassFeature_TitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.ClassFeature_DescriptionRequired); vm.Valid = false; }
            else if (Mode == 1 && ClassFeatureIcon == null) { sb.Append(Resources.BusinessPanel.ClassFeature_IconRequired); vm.Valid = false; }
            else if (ClassFeatureIcon != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ClassFeatureIcon.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (ClassFeatureIcon.ContentLength > 2 * 1024 * 1024) // 2 MB
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

    public class ClassFeature_VM
    {
        public long Id { get; set; }
        public long ClassId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string ClassFeatureIconWithPath { get; set; }
        public string FeatureIconWithPath { get; set; }


    }
}