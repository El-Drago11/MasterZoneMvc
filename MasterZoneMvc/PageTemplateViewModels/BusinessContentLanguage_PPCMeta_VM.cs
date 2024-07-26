using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Web;
using static Google.Apis.Requests.BatchRequest;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinessContentLanguage_PPCMeta_VM
    {
        public long Id { get; set; }
        public long ProfilePageTypeId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long LanguageId { get; set; }
        public int Mode { get; set; }
        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.ValidInformationMessage;
            }

            StringBuilder sb = new StringBuilder();

            if (LanguageId <= 0) { sb.Append(Resources.BusinessPanel.BusinessCategoryRequired); vm.Valid = false; }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }
    public class BusinessContentLanguage_VM
    {
        public long LanguageId { get; set; }
    }

    public class BusinessContentLanguageDetail_VM
    {
        public long Id { get; set; }
        public long LanguageId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public string BusinessOwnerName { get; set; }
        public string Language { get; set; }
        public string LanguageIcon { get; set; }
        public string LanguageIconWithPath { get; set; }
        public int Mode { get; set; }
    }
    public class BusinessLanguageId_VM
    {
       public string LanguageName { get; set; }
    }
}