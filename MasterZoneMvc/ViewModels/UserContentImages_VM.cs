using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class UserContentImages_VM
    {
            public long Id { get; set; }
            public string ImageTitle { get; set; }
            public string Image { get; set; }
            public int Mode { get; set; }
            public HttpPostedFile ImageThumbnail { get; set; }

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
                 if (String.IsNullOrEmpty(ImageTitle)) { sb.Append(Resources.VisitorPanel.UserImageTitleRequired); vm.Valid = false; }
                if (Mode == 1)
                {
                    if (ImageThumbnail == null) { sb.Append(Resources.VisitorPanel.UserImageThumbnailImageRequired); vm.Valid = false; }
                    if (ImageThumbnail != null)
                    {
                        // Validate Uploaded Image File
                        bool isValidImage = true;
                        string[] validImageTypes = new string[] { "image/jpeg", "image/png" };

                        if (!validImageTypes.Contains(ImageThumbnail.ContentType))
                        {
                            isValidImage = false;
                            sb.Append(Resources.VisitorPanel.ValidImageFileType_ErrorMessage);
                        }
                        else if (ImageThumbnail.ContentLength > 2 * 1024 * 1024) // 2 MB
                        {
                            isValidImage = false;
                            sb.Append(String.Format(Resources.VisitorPanel.ValidFileSize_ErrorMessage, "2 MB"));
                        }

                        vm.Valid = isValidImage;
                    }
                }


                if (vm.Valid == false)
                {
                    vm.Message = sb.ToString();
                }

                return vm;
            }
        }
    public class UserContentImagesDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ImageTitle { get; set; }
        public string Image { get; set; }
        public int Mode { get; set; }
        public string ImageThumbNailImageWithPath { get; set; }
    }
    }