﻿using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentVideos_PPCMetaViewModel
    {
       
            public long Id { get; set; }
            public long UserLoginId { get; set; }
            public string Title { get; set; }
            public string VideoDescription { get; set; }
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
                if (String.IsNullOrEmpty(Title)) { sb.Append(Resources.BusinessPanel.TitleRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(VideoDescription)) { sb.Append(Resources.BusinessPanel.VideoDescriptionRequired); vm.Valid = false; }


                if (vm.Valid == false)
                {
                    vm.Message = sb.ToString();
                }

                return vm;
            }

        }
        public class BusinessContentVideos_PPCMetaDetail
        {
            public long Id { get; set; }
            public long UserLoginId { get; set; }
            public string Title { get; set; }
            public string VideoDescription { get; set; }
            public long BusinessOwnerLoginId { get; set; }
            public string VideoTitle { get; set; }
            public string VideoLink { get; set; }
            public string VideoThumbnail { get; set; }
            public string VideoThumbNailImageWithPath { get; set; }
        }
    }
