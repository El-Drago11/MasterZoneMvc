using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class CustomFormElementViewModel
    {
        public long Id { get; set; }
        public long CustomFormId { get; set; }
        public string CustomFormElementName { get; set; }
        public string CustomFormElementType { get; set; }
        public string CustomFormElementValue { get; set; }
        public string CustomFormElementPlaceholder { get; set; }
        public int CustomFormElementStatus { get; set; }
        public int Mode { get; set; }
    }
}