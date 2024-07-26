using MasterZoneMvc.Common.ValidationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Ink;

namespace MasterZoneMvc.ViewModels
{
    public class ApartmentBookingViewModel
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        public string BlockName { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        public string AreaName { get; set; }
        public string Activity { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedByLoginId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedByLoginId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
    }

    public class RequestApartmentBooking_VM
    {
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        public long ApartmentBlockId { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        public long ApartmentAreaId { get; set; }
        public string Activity { get; set; }
        public HttpPostedFile PersonProfileImageFile { get; set; }

        public int PersonHasMasterId { get; set; }
        public string PersonMasterId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string PersonEmail { get; set; }
        public string PersonPhoneNumber { get; set; }
        public int PersonGender { get; set; }

        public List<RequestFamilyMember_VM> FamilyMemberList { get; set; }
        public int Mode { get; set; }

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
            if (ApartmentId <= 0) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Apartment_Required); vm.Valid = false; }
            if (BatchId <= 0) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Batch_Required); vm.Valid = false; }
            if (ApartmentBlockId <= 0) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Block_Required); vm.Valid = false; }
            else if (String.IsNullOrEmpty(FlatOrVillaNumber)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_FlatVillaNum_Required); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Phase)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Phase_Required); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Lane)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Lane_Required); vm.Valid = false; }
            else if (String.IsNullOrEmpty(OccupantType)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_OccupantType_Required); vm.Valid = false; }
            else if (ApartmentAreaId <= 0) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Area_Required); vm.Valid = false; }
            else if (String.IsNullOrEmpty(Activity)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_Activity_Required); vm.Valid = false; }
            
            else if (PersonHasMasterId == 1 && String.IsNullOrEmpty(PersonMasterId)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonMasterId_Required); vm.Valid = false; } 
            else if (PersonHasMasterId == 0 && String.IsNullOrEmpty(PersonFirstName)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonFirstNameRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && String.IsNullOrEmpty(PersonLastName)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonLastNameRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && String.IsNullOrEmpty(PersonEmail)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonEmailRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && !EmailValidationHelper.IsValidEmailFormat(PersonEmail)) {sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonValidEmailRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && String.IsNullOrEmpty(PersonPhoneNumber)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonPhoneNumberRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && !PhoneNumberValidationHelper.IsValidPhoneNumber(PersonPhoneNumber)) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonValidPhoneNumberRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && PersonGender <= 0) { sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonGenderRequired); vm.Valid = false; }
            else if (PersonHasMasterId == 0 && PersonProfileImageFile != null)
            {
                // Validate Uploded Image File
                bool isValidImage = true;
                string[] validImageTypes = new string[] { "image/jpeg", "image/png" };
                if (!validImageTypes.Contains(PersonProfileImageFile.ContentType))
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ValidImageTypesRequired);
                }
                else if (PersonProfileImageFile.ContentLength > 10 * 1024 * 1024) // 10 MB
                {
                    isValidImage = false;
                    sb.Append(Resources.BusinessPanel.ApartmentBooking_PersonProfileImage_ErrorMessage + " " + String.Format(Resources.BusinessPanel.FileSizeRequired, "10 MB"));
                }

                vm.Valid = isValidImage;
            }
            else if(PersonHasMasterId == 0 && FamilyMemberList.Count() > 0)
            {
                // validate Family Member data
                bool isValid = true;

                foreach(var familyMember in FamilyMemberList)
                {
                    Error_VM error_VM = familyMember.ValidInformation();
                    if(error_VM != null && !error_VM.Valid) { 
                        isValid = false; 
                        break;
                    }
                }

                vm.Valid = isValid;
            }

            if (vm.Valid == false)
            {
                vm.Message = sb.ToString();
            }
            
            return vm;
        }
    }



    public class ApartmentBooking_Pagination_SQL_Params_VM
    {
        public Int64 Id { get; set; }
        public Int64 LoginId { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public int Mode { get; set; }
        public JqueryDataTableParamsViewModel JqueryDataTableParams { get; set; }
    }

    public class ApartmentBooking_ForBO_Pagination_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        public string BlockName { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        public string AreaName { get; set; }
        public string Activity { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public string ApartmentName { get; set; }
        public string ClassName { get; set; }
        public string BatchName { get; set; }
        public string PersonFullName { get; set; }
        public string PersonProfileImage { get; set; }
        public string PersonProfileImageWithPath { get; set; }

        public long TotalRecords { get; set; }
        public long SerialNumber { get; set; }
    }

    public class ApartmentBookingDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        public string BlockName { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        public string AreaName { get; set; }
        public string Activity { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public string ApartmentName { get; set; }
        public string ClassName { get; set; }
        public string BatchName { get; set; }
        public string PersonFullName { get; set; }
        public string PersonProfileImage { get; set; }
        public string PersonProfileImageWithPath { get; set; }


        public string BatchScheduledEndOnTime_24HF { get; set; }
        public string BatchScheduledStartOnTime_24HF { get; set; }
        public int ClassIsPaid { get; set; }
        public string ClassPriceType { get; set; }
        public decimal ClassPrice { get; set; }
        public string ClassDays { get; set; }
        public string ClassImage { get; set; }
        public string ClassImageWithPath { get; set; }
        public string ClassDays_ShortForm { get; set; }
        public int ClassDurationSeconds { get; set; }
        public string DayAbbr { get; set; }

        public long InstructorLoginId { get; set; }
        public string InstructorFirstName { get; set; }
        public string InstructorLastName { get; set; }
        public string InstructorFullName { get; set; }
        public string InstructorProfileImageWithPath { get; set; }
        public string InstructorBusinessCategoryName { get; set; }
        public int InstructorIsCertified { get; set; }

        public List<UserFamilyMemberRelation_VM> FamilyMembers { get; set; }

    }
    
    public class ApartmentBookingUserDetail_VM
    {
        public long Id { get; set; }
        public long BusinessOwnerLoginId { get; set; }
        public long UserLoginId { get; set; }
        public string MasterId { get; set; }
        public long BatchId { get; set; }
        public long ApartmentId { get; set; }
        public string BlockName { get; set; }
        public string FlatOrVillaNumber { get; set; }
        public string Phase { get; set; }
        public string Lane { get; set; }
        public string OccupantType { get; set; }
        public string AreaName { get; set; }
        public string Activity { get; set; }
        public DateTime CreatedOn { get; set; }
        public long ApartmentBlockId { get; set; }
        public long ApartmentAreaId { get; set; }

        public string CreatedOn_FormatDate { get; set; }
        public string ApartmentName { get; set; }
        public string PersonFullName { get; set; }
        public string PersonProfileImage { get; set; }
        public string PersonProfileImageWithPath { get; set; }

    }
}