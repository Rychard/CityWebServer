using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityWebServer.Extensibility
{
    public interface IPluginInfo
    {
        /// <summary>
        /// The name of this plugin. Shown in the index page as a link.
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// The author of this plugin (shown in the index page).
        /// </summary>
        string PluginAuthor { get; }

        /// <summary>
        /// The ID/slug for this plugin. Also used as the root for all request handlers provided by this plugin.
        /// </summary>
        string PluginID { get; }
    }
}
