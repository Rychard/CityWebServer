using System;
using System.Net;

namespace CityWebServer.Extensibility
{
    /// <summary>
    /// Represents a handler for servicing requests received by the web server.
    /// </summary>
    public interface IRequestHandler
    {
        /// <summary>
        /// Gets a unique identifier for this handler.  Only one handler can be loaded with a given identifier.
        /// </summary>
        Guid HandlerID { get; }

        /// <summary>
        /// Gets the priority of this request handler.  A request will be handled by the request handler with the lowest priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the display name of this request handler.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Gets the author of this request handler.
        /// </summary>
        String Author { get; }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.  Your class is responsible for handling requests at this path.
        /// </summary>
        /// <remarks>
        /// When set to a value other than <c>null</c>, the Web Server will show this url as a link on the home page.
        /// </remarks>
        String MainPath { get; }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        Boolean ShouldHandle(HttpListenerRequest request);

        /// <summary>
        /// Handles the specified request.  The returned value is written to the response stream.
        /// </summary>
        String Handle(HttpListenerRequest request);
    }
}
