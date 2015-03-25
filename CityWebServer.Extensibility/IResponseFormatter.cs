using System.Net;

namespace CityWebServer.Extensibility
{
    public abstract class IResponseFormatter
    {
        public abstract void WriteContent(HttpListenerResponse response);
    }
}