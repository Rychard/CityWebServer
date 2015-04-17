using System;
using System.Net;
using CityWebServer.Extensibility.Responses;

namespace CityWebServer.Extensibility
{
    public abstract class RequestHandlerBase : IRequestHandler, ILogAppender
    {
        #region ILogAppender Implementation

        public event EventHandler<LogAppenderEventArgs> LogMessage;

        protected void OnLogMessage(String message)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogAppenderEventArgs(message));
            }
        }

        #endregion ILogAppender Implementation

        protected readonly IWebServer _server;
        protected String _mainPath;

        private RequestHandlerBase()
        {
        }

        protected RequestHandlerBase(IWebServer server, String mainPath)
        {
            _server = server;
            _mainPath = mainPath;
        }

        /// <summary>
        /// Gets the server that is currently servicing this instance.
        /// </summary>
        public virtual IWebServer Server { get { return _server; } }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.  Your class is responsible for handling requests at this path.
        /// </summary>
        public virtual String MainPath { get { return _mainPath; } }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public virtual Boolean ShouldHandle(HttpListenerRequest request, String slug)
        {
            return (request.Url.AbsolutePath.Equals(String.Format("/{0}{1}", slug, _mainPath), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        public abstract IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot);

        /// <summary>
        /// Returns a response in JSON format.
        /// </summary>
        protected IResponseFormatter JsonResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new JsonResponseFormatter<T>(content, statusCode);
        }

        /// <summary>
        /// Returns a response in HTML format.
        /// </summary>
        protected IResponseFormatter HtmlResponse(String content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new HtmlResponseFormatter(content, statusCode);
        }

        /// <summary>
        /// Returns a response in plain text format.
        /// </summary>
        protected IResponseFormatter PlainTextResponse(String content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new PlainTextResponseFormatter(content, statusCode);
        }
    }
}