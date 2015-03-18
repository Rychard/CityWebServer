using System;
using System.Net;
using CityWebServer.Extensibility;

namespace CityWebServer.RequestHandlers
{
    public class TestHandler : IRequestHandler, ILogAppender
    {
        public event EventHandler<LogAppenderEventArgs> LogMessage;
        private void OnLogMessage(String message)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogAppenderEventArgs(message));
            }
        }

        public Guid HandlerID
        {
            get { return new Guid("e7019de3-ac6c-4099-a384-a1de325b4a9d"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Test"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Test"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Test", StringComparison.OrdinalIgnoreCase));
        }

        public string Handle(HttpListenerRequest request)
        {
            // Returns an always changing value, useful for testing.
            return DateTime.Now.ToFileTime().ToString();
        }
    }
}
