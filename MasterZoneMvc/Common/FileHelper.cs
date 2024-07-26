using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.Common
{
    public class FileHelper
    {
        /// <summary>
        /// Generate New-File-Name with the Timestamp/GUID
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GenerateFileNameTimeStamp(HttpPostedFile file)
        {
            string fname = file.FileName;

            fname = Path.GetFileNameWithoutExtension(fname);
            // Replace spaces with underscores
            fname = fname.Replace(" ", "_");

            //To Get File Extension  
            string FileExtension = Path.GetExtension(file.FileName);

            //Add Current Date TimeStamp To Attached File Name  
            fname = fname.Trim() + "_" + Guid.NewGuid().ToString().Replace("-", "") + FileExtension;
            return fname;
        }

        /// <summary>
        /// Generate New-File-Name with the Timestamp/GUID
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="extension">File Extension</param>
        /// <returns></returns>
        public static string GenerateFileNameTimeStamp(string fileName, string extension)
        {
            string fname = fileName;

            // Replace spaces with underscores
            fname = fname.Replace(" ", "_");

            //To Get File Extension  
            string FileExtension = extension;

            //Add Current Date TimeStamp To Attached File Name  
            fname = fname.Trim() + "_" + Guid.NewGuid().ToString().Replace("-", "") + FileExtension;
            return fname;
        }

        /// <summary>
        /// Get Server Path
        /// </summary>
        /// <param name="virtualPath">Virtual Path from root to combine</param>
        /// <returns>Server Path with virtual path</returns>
        public static string GetServerPath(string virtualPath)
        {
            // Get the physical path using System.Web.Hosting if available
            if (System.Web.Hosting.HostingEnvironment.IsHosted)
            {
                return System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
            }

            // If not hosted, use a custom configuration or environment variable
            string serverPath = ConfigurationManager.AppSettings["ServerPath"] ?? Environment.GetEnvironmentVariable("SERVER_PATH");

            // Ensure server path ends with a backslash
            if (!serverPath.EndsWith("\\"))
            {
                serverPath += "\\";
            }

            return Path.Combine(serverPath, virtualPath);
        }

        /// <summary>
        /// Save the File passed on the server
        /// </summary>
        /// <param name="file">Http Posted File object</param>
        /// <param name="ImagePath">Image Path on Server with ImageName and extension combined</param>
        /// <returns></returns>
        public string SaveUploadedFile(HttpPostedFile file, string ImagePath)
        {

            // Get the complete folder path and store the file inside it.  
            //var fnameWithPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fname);
            // string fnameWithPath = Path.Combine(HttpContext.Current.Server.MapPath(ImagePath), fileName);
            file.SaveAs(ImagePath);

            return "";
        }

        /// <summary>
        /// Delete File from the server by the File-With-Path-On-Server
        /// </summary>
        /// <param name="fileNameWithPath">File with path on server</param>
        /// <returns>True if deleted</returns>
        public bool DeleteAttachedFileFromServer(string fileNameWithPath)
        {
            // var fullPath = HttpContext.Current.Server.MapPath(fileNameWithPath);
            if (!String.IsNullOrEmpty(fileNameWithPath))
            {
                if (System.IO.File.Exists(fileNameWithPath))
                {
                    System.IO.File.Delete(fileNameWithPath);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove Prevoius file if not empty and upload new image on server
        /// </summary>
        /// <param name="fileServerPath">File Path/directory on Server</param>
        /// <param name="previousFileName">Previous File Name to match in Server Path</param>
        /// <param name="newFileName">New File Name to save</param>
        /// <param name="file">File to save</param>
        public void InsertOrDeleteFileFromServer(string fileServerPath, string previousFileName, string newFileName, HttpPostedFile file)
        {
            // if no new file is passed then return
            if (file == null)
            {
                return;
            }

            // remove previous file
            if (!String.IsNullOrEmpty(previousFileName))
            {
                string RemoveFileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(fileServerPath), previousFileName);
                DeleteAttachedFileFromServer(RemoveFileWithPath);
            }

            // save new file
            string FileWithPath = Path.Combine(HttpContext.Current.Server.MapPath(fileServerPath), newFileName);
            SaveUploadedFile(file, FileWithPath);
        }
    }
}