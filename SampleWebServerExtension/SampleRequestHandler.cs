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
            : base(server, "/")
        {
        }

        public override IResponseFormatter Handle(HttpListenerRequest request, string slug, string wwwroot)
        {
            const String content = "This is a sample page!";

            return HtmlResponse(content);
        }
    }
}