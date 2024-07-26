using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentPortfolio_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PortfolioImage { get; set; }
        public string AudioImage { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentPortfolio_Params_VM ()
        {
            Title = String.Empty;
            Description = String.Empty;
            PortfolioImage = String.Empty;
            AudioImage = String.Empty;
        }
           
    }

    public class SP_InsertUpdateBusinessContentAudio_Params_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }     
        public string AudioFile { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentAudio_Params_VM()
        {
            Title = String.Empty;
            ArtistName = String.Empty;
           
            AudioFile = String.Empty;

        }
    }
}