using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityWebServer.Extensibility
{
    public interface ICityWebPlugin : IPluginInfo
    {
        /// <summary>
        /// Called when the server first starts and is registering handlers for this plugin. Returns an enumerable collection of IRequestHandler objects.
        /// </summary>
        /// <remarks>
        /// Each of the returned handlers will be used by the server to handle requests. Handlers show up on the main index page of the server.
        /// </remarks>
        List<IRequestHandler> GetHandlers(IWebServer server);
    }
}
