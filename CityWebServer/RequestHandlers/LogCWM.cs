using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using CityWebServer.Helpers;
using CityWebServer.Extensibility;
using CityWebServer.Extensibility.Responses;

namespace CityWebServer.RequestHandlers
{
    public class LogCWM : CityWebMod
    {
        public LogCWM(IWebServer server)
        {
            _ID = "log";
            _name = "Server Log";
            _author = "Rychard";
            _isEnabled = true;
            _wwwroot = server.WebRoot;
            _topMenu = true;

            _handlers = new List<IRequestHandler>();
            _handlers.Add(new LogRequestHandler(server));
        }

        private class LogRequestHandler : RequestHandlerBase
        {
            public LogRequestHandler(IWebServer server)
                : base(server, "/")
            {
            }

            public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
            {
                String body = String.Format("<pre>{0}</pre>", String.Join("", _server.LogLines.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "Log", _server.Mods, body);
                var template = TemplateHelper.PopulateTemplate("content", wwwroot, tokens);

                return new HtmlResponseFormatter(template);
            }
        }

    }
}
