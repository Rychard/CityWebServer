using System;
using ICities;

namespace CityWebServer
{
    public class UserModInfo : IUserMod
    {
        public String Name
        {
            get { return "Integrated Web Server"; }
        }

        public String Description
        {
            get { return "Host a web-server allowing you to communicate with the game via a web-browser."; }
        }
    }
}