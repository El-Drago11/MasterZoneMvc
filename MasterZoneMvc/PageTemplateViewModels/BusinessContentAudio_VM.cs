using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentAudio_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public HttpPostedFile AudioFile { get; set; }
        public int Mode { get; set; }
    }


    public class BusinessContentAudioDetail_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
       
        public string AudioFile { get; set; }
       
        public string AudioFileWithPath { get; set; }
        public int Mode { get; set; }
    }
    public class BusinessContentAudioDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }     
        public string AudioFile { get; set; }
        public string AudioFileWithPath { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class BusinessContentAudioDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }
}