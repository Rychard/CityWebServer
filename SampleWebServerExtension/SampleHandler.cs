using System;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;

namespace SampleWebServerExtension
{
    public class SampleHandler : BaseHandler
    {
        public override Guid HandlerID
        {
            get { return new Guid("1a255904-bf72-406e-b5e2-c5a43fdd9bba"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override string Name
        {
            get { return "Sample"; }
        }

        public override string Author
        {
            get { return "Rychard"; }
        }

        public override string MainPath
        {
            get { return "/Sample"; }
        }

        public override bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Sample", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponse Handle(HttpListenerRequest request)
        {
            const String content = "This is a sample page!";

            return HtmlResponse(content);
        }
    }
}
