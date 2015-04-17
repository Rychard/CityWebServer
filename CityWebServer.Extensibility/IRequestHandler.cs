using System;
using System.Net;

namespace CityWebServer.Extensibility
{
    /// <summary>
    /// Represents a handler for servicing requests received by the web server.
    /// </summary>
    public interface IRequestHandler
    {
        IWebServer Server { get; }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.  Your class is responsible for handling requests at this path.
        /// </summary>
        String MainPath { get; }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        Boolean ShouldHandle(HttpListenerRequest request, string slug);

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        IResponseFormatter Handle(HttpListenerRequest request, string slug, string wwwroot);
    }
}