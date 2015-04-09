using System;
using System.Collections.Generic;
using ICities;
using CityWebServer.Extensibility;

namespace SampleWebServerExtension
{
    public class UserModInfo : IUserMod, ICityWebMod
    {
        public String Name
        {
            get { return "Sample Web Server Extension"; }
        }

        public String ModName
        {
            get { return "Sample Extension"; }
        }

        public String ModAuthor
        {
            get { return "Rychard"; }
        }

        public String ModID
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