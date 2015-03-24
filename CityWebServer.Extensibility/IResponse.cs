using System.Net;

namespace CityWebServer.Extensibility
{
    public abstract class IResponse
    {
        public abstract void WriteContent(HttpListenerResponse response);
    }
}