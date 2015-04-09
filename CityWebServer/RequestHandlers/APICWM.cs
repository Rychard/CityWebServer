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
    public class APICWM : CityWebMod
    {
        public APICWM(IWebServer server)
        {
            _ID = "api";
            _name = "JSON API";
            _author = "Rychard";
            _isEnabled = true;
            _wwwroot = null;

            _handlers = new List<IRequestHandler>();
            _handlers.Add(new BudgetRequestHandler(server));
            _handlers.Add(new BuildingRequestHandler(server));
            _handlers.Add(new CityInfoRequestHandler(server));
            _handlers.Add(new MessageRequestHandler(server));
            _handlers.Add(new TransportRequestHandler(server));
            _handlers.Add(new VehicleRequestHandler(server));
        }
    }
}
