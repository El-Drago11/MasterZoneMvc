﻿using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentProtfolio_VM
    {
        public long Id { get; set; }
        public long UerLoginId { get; set; }
        public long ProfilePageType  { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFile PortfolioImage { get; set; }
        public HttpPostedFile AudioImage { get; set; }
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
            //if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.DescriptionRequired); vm.Valid = false; }
             if (PortfolioImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(PortfolioImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (PortfolioImage.ContentLength > 2 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append("Image size too large. Please select a smaller image. (upto 5 MB)");
                }

                vm.Valid = isValidImage;
            }
            else if (AudioImage != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(AudioImage.ContentType))
                {
                    isValidImage = false;
                    sb.Append("Invalid image type. Please select a JPEG, PNG");
                }
                else if (AudioImage.ContentLength > 2 * 1024 * 1024) // 10 MB
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

    public class BusinessContentProtfolioDetail_VM
    {
        public long Id { get; set; }
        public long UerLoginId { get; set; }
        public long ProfilePageType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PortfolioImage { get; set; }
        public string AudioImage { get; set; }
        public string PortfolioImageWithPath { get; set; }
        public string AudioImageWithPath { get; set; }
        public int Mode { get; set; }

    }
}