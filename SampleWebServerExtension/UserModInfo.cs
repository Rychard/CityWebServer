using System;
using System.Collections.Generic;
using ICities;
using CityWebServer.Extensibility;

namespace SampleWebServerExtension
{
    public class UserModInfo : IUserMod, ICityWebPlugin
    {
        public String Name
        {
            get { return "Sample Web Server Extension"; }
        }

        public String PluginName
        {
            get { return "Sample Extension"; }
        }

        public String PluginAuthor
        {
            get { return "Rychard"; }
        }

        public String PluginID
        {
            get { return "sample"; }
        }

        public String Description
        {
            get { return "Adds a sample page to the integrated web server.  Doesn't do anything without it!"; }
        }

        public bool TopMenu {
            get { return true; } 
        }

        public List<IRequestHandler> GetHandlers(IWebServer server)
        {
            List<IRequestHandler> h = new List<IRequestHandler>();
            h.Add(new SampleRequestHandler(server));
            return h;
        }
    }
}