using System;
using System.Collections.Generic;

namespace CityWebServer.Extensibility
{
    public interface IWebServer
    {
        /// <summary>
        /// Gets an array containing all currently registered plugins.
        /// </summary>
        IPluginInfo[] Plugins { get; }

        /// <summary>
        /// Gets the name of the current city.
        /// </summary>
        String CityName { get; }

        /// <summary>
        /// Gets all of the content in the log buffer.
        /// </summary>
        List<String> LogLines { get; }

        /// <summary>
        /// Gets the base static file path for the server.
        /// </summary>
        String WebRoot { get; }
    }
}