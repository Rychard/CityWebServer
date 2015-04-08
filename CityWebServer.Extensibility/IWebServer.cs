using System;
using System.Collections.Generic;

namespace CityWebServer.Extensibility
{
    public interface IWebServer
    {
        /// <summary>
        /// Gets an array containing all currently registered request handlers.
        /// </summary>
        IPluginInfo[] Plugins { get; }
        String CityName { get; }
        List<String> LogLines { get; }
    }
}