﻿using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentFindMasterProfile_ViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string ExploreType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFile Image { get; set; }
        public string ScheduleLink { get; set; }
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
            if (string.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
            else if (string.IsNullOrEmpty(Description)) { sb.Append(Resources.BusinessPanel.SubTitleRequired); vm.Valid = false; }
            else if (string.IsNullOrEmpty(ExploreType)) { sb.Append(Resources.BusinessPanel.ExploreTypeRequired); vm.Valid = false; }
            //else if (string.IsNullOrEmpty(ScheduleLink)) { sb.Append(Resources.BusinessPanel.ScheduleLinkRequired); vm.Valid = false; }
            if (Image != null)
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

    public class BussinessContentFindMasterProfilelDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string ExploreType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ImageWithPath { get; set; }
        public string ScheduleLink { get; set; }
        public int Mode { get; set; }
    }
}