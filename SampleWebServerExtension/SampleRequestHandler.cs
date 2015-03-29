using System;
using System.Net;
using CityWebServer.Extensibility;
using JetBrains.Annotations;

namespace SampleWebServerExtension
{
    [UsedImplicitly]
    public class SampleRequestHandler : RequestHandlerBase
    {
        public SampleRequestHandler(IWebServer server)
            : base(server, new Guid("1a255904-bf72-406e-b5e2-c5a43fdd9bba"), "Sample", "Rychard", 100, "/Sample")
        {
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            const String content = "This is a sample page!";

            return HtmlResponse(content);
        }
    }
}