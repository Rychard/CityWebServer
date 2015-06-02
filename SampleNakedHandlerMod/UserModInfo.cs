using System;
using System.Collections.Generic;
using ICities;

namespace SampleNakedHandlerMod
{
    public class UserModInfo : IUserMod
    {
        public String Name
        {
            get { return "CityWebServer - Sample Naked Handler Mod"; }
        }

        public String Description
        {
            get { return "Adds a sample page to the integrated web server.  Doesn't do anything without it!"; }
        }
    }
}