using MasterZoneMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MasterZoneMvc.Common
{
    /// <summary>
    /// Class to generate License-Certificate and common funcitons for it.
    /// </summary>
    public class LicenseCertificateGenerator
    {
        /// <summary>
        /// Replace Keys with the dynamic values in the License-Certificate Template.
        /// </summary>
        /// <param name="template">License Template HTML eg. Hello, [Name]! Your order was placed on [IssueDate].</param>
        /// <param name="data">Data object</param>
        /// <returns>Replaced Keys with data eg. Output: Hello, John Doe! Your order was placed on 2023-12-27.</returns>
        public static string BindValuesInCertificateHTML(string template, LicenseCertificateHTMLContent_VM data)
        {
            string result = Regex.Replace(template, @"\[(?<key>\w+)\]", match =>
            {
                switch (match.Groups["key"].Value)
                {
                    case "MasterzoneLogoPath":
                        return FileHelper.GetServerPath(Resources.StaticResources.MasterzoneLogo);
                    case "Name":
                        return data.UserFirstName + " " + data.UserLastName;
                    case "IssueDate":
                        return data.IssueDate_Format;
                    case "CertificateNumber":
                        return data.UniqueCertificateNumber;
                    case "CertificateLogoPath":
                        return FileHelper.GetServerPath(data.CertificateLogoPath);
                    case "CertificateTitle":
                        return data.CertificateTitle;
                    case "LicenseTitle":
                        return data.LicenseTitle;
                    case "LicenseLogoPath":
                        return FileHelper.GetServerPath(data.LicenseLogoPath);
                    case "Signature1Path":
                        return FileHelper.GetServerPath(data.Signature1Path);
                    case "Signature2Path":
                        return FileHelper.GetServerPath(data.Signature2Path);
                    case "Signature3Path":
                        return FileHelper.GetServerPath(data.Signature3Path);
                    case "TimePeriod":
                        return data.TimePeriod;
                    default:
                        return match.Value; // Preserve unmatched placeholders
                }
            });

            return result;
        }
    }
}