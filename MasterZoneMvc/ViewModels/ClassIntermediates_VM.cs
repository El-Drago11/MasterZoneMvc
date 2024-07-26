using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class ClassIntermediates_VM
    {
    }
    public class RequestClassIntermediatesImage_VM
    {
        public long Id { get; set; }
        public string ImageTitle { get; set; }
        public string Description { get; set; }
        public HttpPostedFile Image { get; set; }
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
            if (String.IsNullOrEmpty(ImageTitle)) { sb.Append(Resources.BusinessPanel.BusinessImageTitleRequired); vm.Valid = false; }
            else if (Image == null) { sb.Append(Resources.BusinessPanel.BusinessImageRequired); vm.Valid = false; }
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
                else if (Image.ContentLength > 2 * 1024 * 1024) // 2 MB
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

    public class ClassIntermediatesImageResponse_VM
    {
        public long Id { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }

    }

    public class ClassIntermediatesDetail_VM
    {

        public long Id { get; set; }
        public long BusinessOwnerloginId { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string ImageWithPath { get; set; }

    }
    public class SP_ManageClassIntermediatesDetail_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }


    }
}
