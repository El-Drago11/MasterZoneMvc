using iTextSharp.tool.xml.html.head;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MasterZoneMvc.ViewModels;

namespace MasterZoneMvc.ViewModels
{
    public class ApartmentAreaViewModel
    {
        public long Id { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class ApartmentArea_VM
    {
        public long Id { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public string SubTitle { get; set; }
        public string ApartmentImageWithPath { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }

    }

    public class RequestApartmentArea_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long ApartmentId { get; set; }
        public string Name { get; set; }
        public string SubTitle { get; set; }
        public HttpPostedFile ApartmentImage { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int Status { get; set; }
        public long SubmittedByLoginId { get; set; }
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
            if (String.IsNullOrEmpty(Name)) { sb.Append(Resources.BusinessPanel.ApartmentAreaNameRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(SubTitle)) { sb.Append(Resources.BusinessPanel.SubTitleRequired); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
            else if (Price <= 0)
            {
                sb.Append(Resources.BusinessPanel.PriceRequired); vm.Valid = false;
            }
            else if (ApartmentImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(ApartmentImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (ApartmentImage.ContentLength > 10 * 1024 * 1024) // 10 MB
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
}