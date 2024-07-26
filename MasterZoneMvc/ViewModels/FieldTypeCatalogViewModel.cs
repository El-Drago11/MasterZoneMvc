using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class FieldTypeCatalogViewModel
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public long PanelTypeId { get; set; }
        public string KeyName { get; set; }
        public string TextValue { get; set; }
        public int IsActive { get; set; }
    }
}