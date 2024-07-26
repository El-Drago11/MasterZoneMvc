using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class FormElement
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Placeholder { get; set; }

        public List<FormOption> options { get; set; }
        public List<SelectedOption> selectedOptions { get; set; }


    }
    public class FormOption
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class SelectedOption
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }


    public class CustomFormDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long CustomFormElementId { get; set; }
        public long CustomFormOptionId { get; set; }
        public string CustomFormName { get; set; }
        public string CustomFormElementOptions { get; set; }
        public string CustomFormElementName { get; set; }
        public string CustomFormElementType { get; set; }
        public string CustomFormElementValue { get; set; }
        public string CustomFormElementPlaceholder { get; set; }
        public int CustomFormElementStatus { get; set; }

    }


    public class CustomFormDetail_ViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long CustomFormElementId { get; set; }
        public string CustomFormName { get; set; }
        public string CustomFormElementOptions { get; set; }
        public string CustomFormElementName { get; set; }
        public string CustomFormElementType { get; set; }
        public string CustomFormElementValue { get; set; }
        public string CustomFormElementPlaceholder { get; set; }
        public int CustomFormElementStatus { get; set; }

    }

    public class CustomFormOptionsValueDetail
    {
        public string CustomFormElementOptions { get; set; }
    }
}