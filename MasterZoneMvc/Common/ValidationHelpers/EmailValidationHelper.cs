using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace MasterZoneMvc.Common.ValidationHelpers
{
    public class EmailValidationHelper
    {
        private const string emailRegEx = @"^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        
        /// <summary>
        /// Validates Email Address
        /// </summary>
        /// <param name="email">email to validate</param>
        /// <returns>True if valid, False if invalid</returns>
        public static bool IsValidEmailFormat(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            // Create a regular expression to match email addresses.
            var regex = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$", RegexOptions.None);

            // Match the regular expression against the email address.
            var match = regex.Match(email);

            // Return true if the regular expression matched the email address, false otherwise.
            return match.Success;
        }

    }
}