using ICities;

namespace SampleWebServerExtension
{
    public class UserModInfo : IUserMod
    {
        public string Name
        {
            get { return "Sample Web Server Extension"; }
        }

        public string Description
        {
            get { return "Adds a sample page to the integrated web server.  Doesn't do anything without it!"; }
        }
    }
}
