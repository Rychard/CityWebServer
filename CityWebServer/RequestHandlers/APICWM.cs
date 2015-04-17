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
    public class APICWM : CityWebMod
    {
        public APICWM(IWebServer server)
        {
            _ID = "api";
            _name = "JSON API";
            _author = "Rychard";
            _isEnabled = true;
            _wwwroot = server.WebRoot;
            _topMenu = true;

            _handlers = new List<IRequestHandler>();
            _handlers.Add(new APIRequestHandler(server, this));
            _handlers.Add(new BudgetRequestHandler(server));
            _handlers.Add(new BuildingRequestHandler(server));
            _handlers.Add(new CityInfoRequestHandler(server));
            _handlers.Add(new MessageRequestHandler(server));
            _handlers.Add(new TransportRequestHandler(server));
            _handlers.Add(new VehicleRequestHandler(server));
        }

        private class APIRequestHandler : RequestHandlerBase
        {
            private APICWM _container;
            public APIRequestHandler(IWebServer server, APICWM container)
                : base(server, "/")
            {
                _container = container;
            }

            public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
            {
                List<String> links = new List<String>();
                foreach (var h in _container.GetHandlers(_server).OrderBy(obj => obj.MainPath))
                {
                    if (h.MainPath.Equals("/")) { continue; }
                    links.Add(String.Format("<li><a href='/{0}{1}'>{2}</a></li>", slug, h.MainPath, String.IsNullOrEmpty(h.Name) ? h.MainPath : h.Name));
                }

                String body = String.Format("<ul>{0}</ul>", String.Join("", links.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "API Endpoints", _server.Mods, body);
                var template = TemplateHelper.PopulateTemplate("content", wwwroot, tokens);

                return new HtmlResponseFormatter(template);
            }
        }

    }
}
