using ICities;

namespace CityWebServer
{
    public class UserModInfo : IUserMod
    {
        public string Name
        {
            get { return "Integrated Web Server"; }
        }

        public string Description
        {
            get { return "Host a web-server allowing you to communicate with the game via a web-browser."; }
        }
    }
}