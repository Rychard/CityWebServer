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
    public class RootCWM : CityWebMod
    {
        public RootCWM(IWebServer server)
        {
            _ID = "root";
            _name = "Index Page";
            _author = "Rychard";
            _isEnabled = true;
            _wwwroot = server.WebRoot;
            _topMenu = false;

            _handlers = new List<IRequestHandler>();
            _handlers.Add(new RootRequestHandler(server));
        }

        private class RootRequestHandler : RequestHandlerBase
        {
            public RootRequestHandler(IWebServer server)
                : base(server, "/")
            {
            }

            public override IResponseFormatter Handle(HttpListenerRequest request, string slug, string wwwroot)
            {
                List<String> links = new List<String>();
                foreach (var cwm in _server.Mods.OrderBy(obj => obj.ModID))
                {
                    if (!cwm.ModID.Equals("root")) {
                        links.Add(String.Format("<li><a href='{1}/'>{0}</a> by {2}</li>", cwm.ModName, cwm.ModID, cwm.ModAuthor));
                    }
                }

                String body = String.Format("<ul>{0}</ul>", String.Join("", links.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "Home", _server.Mods, body);
                var template = TemplateHelper.PopulateTemplate("index", wwwroot, tokens);

                return new HtmlResponseFormatter(template);
            }

            public override Boolean ShouldHandle(HttpListenerRequest request, string slug)
            {
                string url = request.Url.AbsolutePath;
                return (null != url && (url.Equals("") || url.Equals("/")));
            }
        }
    }
}
