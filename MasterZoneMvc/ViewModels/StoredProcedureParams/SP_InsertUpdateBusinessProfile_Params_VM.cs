using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels.StoredProcedureParams
{
    public class SP_InsertUpdateBusinessProfile_Params_VM
    {
        public Int64 Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public string BusinessLogo { get; set; }
        public string About { get; set; }
        public int Mode { get; set; }
        public string LandMark { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PinCode { get; set; }
        public string PhoneNumber { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentUploadedFile { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string BusinessName { get; set; }
        public int Privacy_UniqueUserId { get; set; }
        public string Experience { get; set; }
        public string OfficialWebSiteUrl { get; set; }
        public string FacebookProfileLink { get; set; }
        public string InstagramProfileLink { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string TwitterProfileLink { get; set; }

        public SP_InsertUpdateBusinessProfile_Params_VM()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
            Email = String.Empty;
            ProfileImage = String.Empty;
            BusinessLogo = String.Empty;
            About = String.Empty;
            Mode = 0;
            LandMark = String.Empty;
            Address = String.Empty;
            City = String.Empty;
            State = String.Empty;
            Country = String.Empty;
            PinCode = String.Empty;
            PhoneNumber = String.Empty;
            DocumentTitle = String.Empty;
            DocumentUploadedFile = String.Empty;
            BusinessName = String.Empty;
            Experience = String.Empty;
            OfficialWebSiteUrl = String.Empty;
            FacebookProfileLink = String.Empty;
            InstagramProfileLink = String.Empty;
            LinkedInProfileLink = String.Empty;
            TwitterProfileLink = String.Empty;
            CoverImage = String.Empty;
        }
    }

}