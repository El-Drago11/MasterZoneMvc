using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels.PageTemplateStoreProcedureViewModel
{
    public class SP_InsertUpdateBusinessContentTennis_Param_VM
    {
        public long Id { get; set; }
        public long SlotId { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string TennisImage { get; set; }
        public string TennisImageWithPath { get; set; }
        public decimal Price { get; set; }
        public decimal OtherPrice { get; set; }
        public decimal CommercialPrice { get; set; }
        public decimal BasicPrice { get; set; }
        public int Mode { get; set; }
        public SP_InsertUpdateBusinessContentTennis_Param_VM()
        {
            Title = String.Empty;
            SubTitle = String.Empty;
            Description = String.Empty;
            TennisImage = String.Empty;
            Price = 0;
            BasicPrice = 0;
            CommercialPrice = 0;
            OtherPrice = 0;
        }            
    }
}