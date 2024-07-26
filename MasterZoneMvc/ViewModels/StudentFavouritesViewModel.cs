using MasterZoneMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterZoneMvc.ViewModels
{
    public class StudentFavouritesViewModel
    {
    }

    public class FavouriteStudents_VM
    {
        public Int64 Id { get; set; }
        public Int64 UserLoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class FavouriteBusiness_VM
    {
        public Int64 FavouriteUserLoginId { get; set; }
        public Int64 StudentLoginId { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageWithPath { get; set; }
        public int FavouritesCount { get; set; }
        public int IsCertified { get; set; }
        public int Verified { get; set; }
        public string VerifiedTextValue { get; set; }
        public decimal AverageRating { get; set; }
        public string BusinessCategoryName { get; set; }
        public string BusinessSubCategoryName { get; set; }
        public List<UserCertificateBasicInfo_VM> Certifications { get; set; }
    }

    public class FavouriteInstructor_VM
    {
        public Int64 FavouriteUserLoginId { get; set; }
        public string BusinessName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessProfileImageWithPath { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    public class UserInstructor_VM
    {
        public long Id { get; set; }
        public string InstructorName { get; set; }
        public long FavouriteUserLoginId { get; set; }
        public string ProfileImage { get; set; }
    }
}