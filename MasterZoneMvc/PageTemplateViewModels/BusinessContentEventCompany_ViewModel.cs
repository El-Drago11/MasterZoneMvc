using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentEventCompany_ViewModel
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFile Image { get; set; }
        public string EventOptions { get; set; }
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

            if (Image != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(Image.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (Image.ContentLength > 2 * 1024 * 1024) // 10 MB
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

    public class BusinessContentEventCompanyDetail_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }

        public long BusinessOwnerLoginId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string EventCompanyImageWithPath { get; set; }
        public string EventOptions { get; set; }
        public int Mode { get; set; }
    }
    public class BusinessContentEventCompanyDetailEventOptions_VM
    {
        public string EventOptionsName { get; set; }
    }
}