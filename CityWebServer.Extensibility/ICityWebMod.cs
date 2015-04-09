using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityWebServer.Extensibility
{
    public interface ICityWebMod
    {
        /// <summary>
        /// The name of this mod. Shown in the index page/nav menu as a link.
        /// </summary>
        string ModName { get; }

        /// <summary>
        /// The author of this mod.
        /// </summary>
        string ModAuthor { get; }

        /// <summary>
        /// The ID/slug for this mod.
        /// </summary>
        /// <remarks>
        /// This must be unique across all CityWebMods, and is used as the root for all request handlers provided by this mod.
        /// </remarks>
        string ModID { get; }

        /// <summary>
        /// Whether this mod should show up in the top menu.
        /// </summary>
        bool TopMenu { get; }

        /// <summary>
        /// Called when the server first starts and is registering handlers. Returns an enumerable collection of IRequestHandler objects.
        /// </summary>
        List<IRequestHandler> GetHandlers(IWebServer server);
    }
}
