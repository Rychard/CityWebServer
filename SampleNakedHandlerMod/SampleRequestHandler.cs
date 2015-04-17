﻿using System;
using System.Net;
using CityWebServer.Extensibility;
using JetBrains.Annotations;

namespace SampleNakedHandlerMod
{
    [UsedImplicitly]
    public class SampleRequestHandler : RequestHandlerBase
    {
        public SampleRequestHandler(IWebServer server)
            : base(server, "Sample Naked Handler", "Rychard", 100, "/samplenaked")
        {
        }

        public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
        {
            const String content = "This is a sample page!";

            return HtmlResponse(content);
        }
    }
}