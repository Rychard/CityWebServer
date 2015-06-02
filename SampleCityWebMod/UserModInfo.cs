using System;
using System.Collections.Generic;
using ICities;
using CityWebServer.Extensibility;

namespace SampleCityWebMod
{
    public class UserModInfo : IUserMod, ICityWebMod
    {
        public String Name
        {
            get { return "CityWebServer - Sample CityWebMod"; }
        }

        public String ModName
        {
            get { return "Sample CWM"; }
        }

        public String ModAuthor
        {
            get { return "nezroy"; }
        }

        public String ModID
        {
            get { return "samplecwm"; }
        }

        public String Description
        {
            get { return "Adds a sample page to the integrated web server.  Doesn't do anything without it!"; }
        }

        public Boolean TopMenu {
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