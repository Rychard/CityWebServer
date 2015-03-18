using System;
using System.Net;
using CityWebServer.Extensibility;

namespace SampleWebServerExtension
{
    public class SampleHandler : IRequestHandler
    {
        public Guid HandlerID
        {
            get { return new Guid("1a255904-bf72-406e-b5e2-c5a43fdd9bba"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Sample"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Sample"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Sample", StringComparison.OrdinalIgnoreCase));
        }

        public string Handle(HttpListenerRequest request)
        {
            return "This is a sample page!";
        }
    }
}
