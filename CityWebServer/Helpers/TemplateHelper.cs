using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CityWebServer.Extensibility;

namespace CityWebServer.Helpers
{
    public static class TemplateHelper
    {
        /// <summary>
        /// Gets the full content of a template.
        /// </summary>
        public static String GetTemplate(String template, String tmplPath)
        {
            // Templates seem like something we shouldn't handle internally.
            // Perhaps we should force request handlers to implement their own templating if they so desire, and maintain a more "API" approach within the core.
            String specifiedTemplatePath = String.Format("{0}{1}{2}.html", tmplPath, Path.DirectorySeparatorChar, template);

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
        public static String PopulateTemplate(String template, String tmplPath, Dictionary<String, String> tokenReplacements)
        {
            try
            {
                String templateContents = GetTemplate(template, tmplPath);
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
        public static Dictionary<String, String> GetTokenReplacements(String cityName, String title, IPluginInfo[] plugins, String body)
        {
            var handlerLinks = plugins.Select(obj => obj.TopMenu ? String.Format("<li><a href='/{0}/'>{1}</a></li>", obj.PluginID, obj.PluginName) : "").ToArray();
            String nav = String.Join(Environment.NewLine, handlerLinks);

            return new Dictionary<String, String>
            {
                { "#PAGETITLE#", title },
                { "#NAV#", nav},
                { "#PAGEBODY#", body},
                { "#CITYNAME#", cityName},
            };
        }
    }
}