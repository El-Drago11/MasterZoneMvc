using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;

namespace MasterZoneMvc.Common.Helpers
{
    public class ResourcesHelper
    {
        public static string GetResourceValue(string fileName, string key)
        {

            string resourceValue = string.Empty;

            ResourceManager resourceManager = new ResourceManager("MasterZoneMvc.App_GlobalResources." + fileName, Assembly.GetExecutingAssembly());

            // Retrieve the resource value for the specified key name
            try
            {

                // CultureInfo cultureInfo = new CultureInfo("hi-IN");
                //Thread.CurrentThread.CurrentUICulture = cultureInfo;
                //Thread.CurrentThread.CurrentCulture = cultureInfo;
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                //resourceValue = resourceManager.GetString(key,  cultureInfo);

                resourceValue = resourceManager.GetString(key);
            }
            catch (MissingManifestResourceException)
            {
                // Handle the exception if the resource file or key name is not found
                throw;
            }

            return resourceValue;
        }


    }
}