using MasterZoneMvc.Models;
using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace MasterZoneMvc.PageTemplateViewModels
{
    public class BusinressContentStudioEquipment_VM
    {
       
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentValue { get; set; }
        public int Mode  { get; set; }

        public Error_VM ValidInformation()
        {
            var vm = new Error_VM();
            vm.Valid = true;

            if (this == null)
            {
                vm.Valid = false;
                vm.Message = Resources.ErrorMessage.InvalidData_ErrorMessage;
                return vm;
            }

            StringBuilder sb = new StringBuilder();
            //if (String.IsNullOrEmpty(EquipmentType)) { sb.Append(Resources.BusinessPanel.EquipmentTypeRequired); vm.Valid = false; }
            //else if (String.IsNullOrEmpty(EquipmentType)) { sb.Append(Resources.BusinessPanel.EquipmentValueRequired); vm.Valid = false; }
            if (String.IsNullOrEmpty(EquipmentType)) { sb.Append("EquipmentTypeRequired"); vm.Valid = false; }
            else if (String.IsNullOrEmpty(EquipmentType)) { sb.Append("EquipmentValueRequired"); vm.Valid = false; }


            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }

    }

    public class BusinressContentStudioEquipmentDetail_VM
    {

        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentValue { get; set; }
        public int Mode { get; set; }
    }
    public class BusinressContentStudioEquipmentValueDetail_VM
    {
        public string EquipmentValueName { get; set; }
    }

    public class BusinessContentStudioEqipment_PPCMeta_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Mode { get; set; }
    }

    public class BusinessContentStudioEqipmentDetail_Pagination_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public long ProfilePageTypeId { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentValue { get; set; }
        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }

    }
    public class BusinessContentStudioEqipmentDetail_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public Int64 BusinessAdminLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

}