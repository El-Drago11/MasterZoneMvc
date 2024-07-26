using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MasterZoneMvc.Common.ValidationHelpers
{
    public class PhoneNumberValidationHelper
    {
        private const string phoneNumberRegEx = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

        /// <summary>
        /// Validates Phone Number
        /// </summary>
        /// <param name="phoneNumber">phone number to validate</param>
        /// <returns>True if valid, False if invalid</returns>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (!String.IsNullOrEmpty(phoneNumber))
                return Regex.IsMatch(phoneNumber, phoneNumberRegEx);
            return false;
        }

    }
}