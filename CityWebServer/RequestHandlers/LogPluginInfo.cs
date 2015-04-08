using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using ColossalFramework.Plugins;
using CityWebServer.Helpers;
using CityWebServer.Extensibility;
using CityWebServer.Extensibility.Responses;

namespace CityWebServer.RequestHandlers
{
    public class LogPluginInfo : CityWebPluginInfo
    {
        public LogPluginInfo(IWebServer server)
        {
            _ID = "log";
            _name = "System Log";
            _author = "Rychard";
            _isEnabled = true;
            _wwwroot = null;

            _handlers = new List<IRequestHandler>();
            _handlers.Add(new LogRequestHandler(server));
        }

        private class LogRequestHandler : RequestHandlerBase
        {
            public LogRequestHandler(IWebServer server)
                : base(server, "/")
            {
            }

            public override IResponseFormatter Handle(HttpListenerRequest request)
            {
                String body = String.Format("<h1>Server Log</h1><pre>{0}</pre>", String.Join("", _server.LogLines.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "Log", _server.Plugins, body);
                var template = TemplateHelper.PopulateTemplate("index", tokens);

                return new HtmlResponseFormatter(template);
            }
        }

    }
}
