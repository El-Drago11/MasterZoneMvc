using MasterZoneMvc.Common;
using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class FamilyMemberViewModel
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relation { get; set; }
        public int Gender { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class RequestFamilyMember_VM
    {
        public long Id { get; set; }
        public string ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FamilyRelationTypeKey { get; set; }
        public int Gender { get; set; }
        public HttpPostedFile ProfileImageFile { get; set; }
        public int FamilyMemberHasMasterId { get; set; }
        public string FamilyMemberMasterId { get; set; }
        public long FamilyMemberUserLoginId { get; set; }
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
            if(FamilyMemberHasMasterId > 0 && String.IsNullOrEmpty(FamilyMemberMasterId)) { sb.Append(Resources.BusinessPanel.FamilyMember_FirstNameRequired); vm.Valid = false; }
            else if(FamilyMemberHasMasterId <= 0)
            {
                if (String.IsNullOrEmpty(FirstName)) { sb.Append(Resources.BusinessPanel.FamilyMember_FirstNameRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(LastName)) { sb.Append(Resources.BusinessPanel.FamilyMember_LastNameRequired); vm.Valid = false; }
                else if (String.IsNullOrEmpty(Email)) { sb.Append(Resources.BusinessPanel.FamilyMember_EmailRequired); vm.Valid = false; }
                else if (!EmailValidationHelper.IsValidEmailFormat(Email)) { sb.Append(Resources.BusinessPanel.ValidEmailRequired + $"({Email})"); vm.Valid = false; }
                else if (String.IsNullOrEmpty(FamilyRelationTypeKey)) { sb.Append(Resources.BusinessPanel.FamilyMember_RelationRequired); vm.Valid = false; }
                else if (Gender <= 0) { sb.Append(Resources.BusinessPanel.FamilyMember_GenderRequired); vm.Valid = false; }
                else if (ProfileImageFile != null)
                {
                    // Validate Uploded Image File
                    bool isValidImage = true;
                    string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                    if (!validImageTypes.Contains(ProfileImageFile.ContentType))
                    {
                        isValidImage = false;
                        sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                    }
                    else if (ProfileImageFile.ContentLength > 10 * 1024 * 1024) // 10 MB
                    {
                        isValidImage = false;
                        sb.Append(String.Format(Resources.BusinessPanel.FamilyMember_ProfileImage_ErrorMessage, FirstName + " " + LastName) + " " + String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                    }

                    vm.Valid = isValidImage;
                }
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }

            return vm;
        }
    }

    public class FamilyMember_VM
    {
        public long Id { get; set; }
        public long UserLoginId { get; set; }
        public string ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relation { get; set; }
        public int Gender { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; } 
        public string ProfileImageWithPath { get; set; }
    }
}