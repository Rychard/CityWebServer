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

            public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
            {
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "Home", _server.Mods, "");
                var template = TemplateHelper.PopulateTemplate("index", wwwroot, tokens);

                return new HtmlResponseFormatter(template);
            }

            public override Boolean ShouldHandle(HttpListenerRequest request, String slug)
            {
                String url = request.Url.AbsolutePath;
                return (null != url && (url.Equals("") || url.Equals("/")));
            }
        }
    }
}
