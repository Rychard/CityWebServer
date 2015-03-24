using System;
using System.Net;
using CityWebServer.Extensibility.Responses;

namespace CityWebServer.Extensibility
{
    public abstract class BaseHandler : IRequestHandler
    {
        /// <summary>
        /// Gets a unique identifier for this handler.  Only one handler can be loaded with a given identifier.
        /// </summary>
        public abstract Guid HandlerID { get; }

        /// <summary>
        /// Gets the priority of this request handler.  A request will be handled by the request handler with the lowest priority.
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// Gets the display name of this request handler.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the author of this request handler.
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.  Your class is responsible for handling requests at this path.
        /// </summary>
        /// <remarks>
        /// When set to a value other than <c>null</c>, the Web Server will show this url as a link on the home page.
        /// </remarks>
        public abstract string MainPath { get; }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public abstract bool ShouldHandle(HttpListenerRequest request);

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        public abstract IResponse Handle(HttpListenerRequest request);

        /// <summary>
        /// Returns a response in JSON format.
        /// </summary>
        protected IResponse JsonResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new JsonResponse<T>(content, statusCode);
        }

        /// <summary>
        /// Returns a response in HTML format.
        /// </summary>
        protected IResponse HtmlResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new HtmlResponse(content, statusCode);
        }

        protected IResponse PlainTextResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new PlainTextResponse(content, statusCode);
        }
    }
}