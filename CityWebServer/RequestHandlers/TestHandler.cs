using System;
using System.Collections.Generic;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;

namespace CityWebServer.RequestHandlers
{
    public class TestHandler : BaseHandler, ILogAppender
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

        public override Guid HandlerID
        {
            get { return new Guid("e7019de3-ac6c-4099-a384-a1de325b4a9d"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override string Name
        {
            get { return "Test"; }
        }

        public override string Author
        {
            get { return "Rychard"; }
        }

        public override string MainPath
        {
            get { return "/Test"; }
        }

        public override bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Test", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponse Handle(HttpListenerRequest request)
        {
            List<String> s = new List<string>();

            s.Add("Test");

            return JsonResponse(s);
        }
    }
}