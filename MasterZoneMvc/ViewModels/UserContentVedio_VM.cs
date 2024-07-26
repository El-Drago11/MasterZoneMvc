using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserContentVedio_VM
    {
        public long Id { get; set; }
         public long UserLoginId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoLink { get; set; }
        public string VideoThumbnail { get; set; }
        public string VideoThumbNailImageWithPath { get; set; }
    }
}