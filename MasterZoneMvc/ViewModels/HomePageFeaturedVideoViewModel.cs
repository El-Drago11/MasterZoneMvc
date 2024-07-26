using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class HomePageFeaturedVideoViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }
    
    public class HomePageFeaturedVideo_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Video { get; set; }
        public int Status { get; set; }
        public string VideoWithPath { get; set; }

    }

    public class RequestHomePageFeaturedVideo_VM
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFile Video { get; set; }
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
            if (Video != null)
            {
                // Validate Uploded Image File
                bool isValidFile = true;

                //string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                //if (!validImageTypes.Contains(Video.ContentType))
                //{
                //    isValidImage = false;
                //    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                //}
                if (Video.ContentLength > 50 * 1024 * 1024) // 50 MB
                {
                    isValidFile = false;
                    sb.Append(String.Format(Resources.BusinessPanel.FileSizeRequired, "50 MB"));
                }

                vm.Valid = isValidFile;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
}