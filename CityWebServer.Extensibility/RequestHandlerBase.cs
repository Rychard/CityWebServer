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
        protected Guid _handlerID;
        protected int _priority;
        protected String _name;
        protected String _author;
        protected String _mainPath;

        private RequestHandlerBase()
        {
        }

        protected RequestHandlerBase(IWebServer server, Guid handlerID, String name, String author, int priority, String mainPath)
        {
            _server = server;
            _handlerID = handlerID;
            _name = name;
            _author = author;
            _priority = priority;
            _mainPath = mainPath;
        }

        /// <summary>
        /// Gets the server that is currently servicing this instance.
        /// </summary>
        public virtual IWebServer Server { get { return _server; } }

        /// <summary>
        /// Gets a unique identifier for this handler.  Only one handler can be loaded with a given identifier.
        /// </summary>
        public virtual Guid HandlerID { get { return _handlerID; } }

        /// <summary>
        /// Gets the priority of this request handler.  A request will be handled by the request handler with the lowest priority.
        /// </summary>
        public virtual int Priority { get { return _priority; } }

        /// <summary>
        /// Gets the display name of this request handler.
        /// </summary>
        public virtual String Name { get { return _name; } }

        /// <summary>
        /// Gets the author of this request handler.
        /// </summary>
        public virtual String Author { get { return _author; } }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.  Your class is responsible for handling requests at this path.
        /// </summary>
        /// <remarks>
        /// When set to a value other than <c>null</c>, the Web Server will show this url as a link on the home page.
        /// </remarks>
        public virtual String MainPath { get { return _mainPath; } }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public virtual Boolean ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals(_mainPath, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        public abstract IResponseFormatter Handle(HttpListenerRequest request);

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