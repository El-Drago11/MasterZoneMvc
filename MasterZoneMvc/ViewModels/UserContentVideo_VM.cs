using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserContentVideo_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string VideoDescription { get; set; }
        public string VideoThumbnail { get; set; }
        public string VideoThumbNailImageWithPath { get; set; }
        public int Mode { get; set; }
        public long TotalRecords { get; set; }
    }
}