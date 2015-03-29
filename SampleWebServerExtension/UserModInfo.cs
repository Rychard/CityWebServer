using System;
using ICities;

namespace SampleWebServerExtension
{
    public class UserModInfo : IUserMod
    {
        public String Name
        {
            get { return "Sample Web Server Extension"; }
        }

        public String Description
        {
            get { return "Adds a sample page to the integrated web server.  Doesn't do anything without it!"; }
        }
    }
}