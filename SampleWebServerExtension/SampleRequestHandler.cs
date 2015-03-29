using System;
using System.Net;
using CityWebServer.Extensibility;

namespace SampleWebServerExtension
{
    public class SampleRequestHandler : RequestHandlerBase
    {
        public override Guid HandlerID
        {
            get { return new Guid("1a255904-bf72-406e-b5e2-c5a43fdd9bba"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override String Name
        {
            get { return "Sample"; }
        }

        public override String Author
        {
            get { return "Rychard"; }
        }

        public override String MainPath
        {
            get { return "/Sample"; }
        }

        public override Boolean ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Sample", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            const String content = "This is a sample page!";

            return HtmlResponse(content);
        }
    }
}