using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MasterZoneMvc.Common.ValidationHelpers
{
    public static class PasswordValidator
    {
        /// <summary>
        /// Value: Password must be atleast 6 character & must contain a special character
        /// </summary>
        public static string PasswordValidationMessage = "Password must be atleast 6 character & must contain a special character";
        
        /// <summary>
        /// Validate Passowrd:
        /// - must be atleast 6 characters long
        /// - must contain a special character
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            if(!String.IsNullOrEmpty(password))
                return Regex.IsMatch(password, @"^(?=.*?[A-Za-z])(?=.*?[#?!@$%^&*-]).{6,}");
            return false;
        }
    }
}