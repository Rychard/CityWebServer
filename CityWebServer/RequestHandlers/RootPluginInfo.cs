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
    public class RootPluginInfo : CityWebPluginInfo
    {
        public RootPluginInfo(IWebServer server)
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

            public override IResponseFormatter Handle(HttpListenerRequest request)
            {
                List<String> links = new List<String>();
                foreach (var plugin in _server.Plugins.OrderBy(obj => obj.PluginID))
                {
                    if (!plugin.PluginID.Equals("root")) {
                        links.Add(String.Format("<li><a href='{1}/'>{0}</a> by {2}</li>", plugin.PluginName, plugin.PluginID, plugin.PluginAuthor));
                    }
                }

                String body = String.Format("<ul>{0}</ul>", String.Join("", links.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "Home", _server.Plugins, body);
                var template = TemplateHelper.PopulateTemplate("index", _server.WebRoot, tokens);

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
