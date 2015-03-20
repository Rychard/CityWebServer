using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CityWebServer.Extensibility;
using ColossalFramework;
using ColossalFramework.IO;

namespace CityWebServer.Helpers
{
    public static class TemplateHelper
    {
        /// <summary>
        /// Gets the full path of the directory that contains this assembly.
        /// </summary>
        /// <remarks>
        /// This is currently rigged to work in a specific manner.
        /// </remarks>
        public static String GetModPath()
        {
            // TODO: Find a better way of obtaining this information.
            String addonRoot = DataLocation.addonsPath;
            String modPath = Path.Combine(addonRoot, "Mods");
            String assemblyPath = Path.Combine(modPath, "CityWebServer_CityWebServer");
            return assemblyPath;
        }

        /// <summary>
        /// Gets the full content of a template.
        /// </summary>
        public static String GetTemplate(String template)
        {
            // Templates seem like something we shouldn't handle internally.
            // Perhaps we should force request handlers to implement their own templating if they so desire, and maintain a more "API" approach within the core.
            String modPath = GetModPath();
            String templatePath = Path.Combine(modPath, "Templates");
            String specifiedTemplatePath = String.Format("{0}{1}{2}.html", templatePath, Path.DirectorySeparatorChar, template);

            if (File.Exists(specifiedTemplatePath))
            {
                String templateContents = File.ReadAllText(specifiedTemplatePath);
                return templateContents;
            }

            // All templates must at least have a #PAGEBODY# token.
            // If we can't find the specified template, just return a string that contains only that.
            return "#PAGEBODY#";
        }

        /// <summary>
        /// Retrieves the template with the specified name, and returns the contents of the template after replacing instances of the dictionary keys from <paramref name="tokenReplacements"/> with their coorresponding values.
        /// </summary>
        /// <param name="template">The name of the template to populate.</param>
        /// <param name="tokenReplacements">A dictionary containing key/value pairs for replacement.</param>
        /// <remarks>
        /// The value of <paramref name="template"/> should not include the file extension.
        /// </remarks>
        public static String PopulateTemplate(String template, Dictionary<String, String> tokenReplacements)
        {
            try
            {
                String templateContents = GetTemplate(template);
                foreach (var tokenReplacement in tokenReplacements)
                {
                    templateContents = templateContents.Replace(tokenReplacement.Key, tokenReplacement.Value);
                }
                return templateContents;
            }
            catch (Exception ex)
            {
                IntegratedWebServer.LogMessage(ex.ToString());
                return tokenReplacements["#PAGEBODY#"];
            }
        }

        /// <summary>
        /// Gets a dictionary that contains standard replacement tokens using the specified values.
        /// </summary>
        public static Dictionary<String, String> GetTokenReplacements(String cityName, String title, List<IRequestHandler> handlers, String body)
        {
            // TODO: Order the request handlers by priority and name?  Whatever the decision is, there needs to be some defined order here.
            String nav = String.Join(Environment.NewLine, handlers.Select(obj => String.Format("<li><a href='{0}'>{1}</a></li>", obj.MainPath, obj.Name)).ToArray());

            return new Dictionary<string, string>
            {
                { "#PAGETITLE#", title },
                { "#NAV#", nav},
                { "#CSS#", ""}, // Moved directly into the template.
                { "#PAGEBODY#", body},
                { "#CITYNAME#", cityName},
                { "#JS#", ""}, // Moved directly into the template.
            };
        }
    }
}
