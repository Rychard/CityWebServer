using System.Net;
using System.Text;

namespace CityWebServer.Extensibility.Responses
{
    internal class PlainTextResponse : IResponse
    {
        private readonly string _content;
        private readonly HttpStatusCode _statusCode;

        public PlainTextResponse(string content, HttpStatusCode statusCode)
        {
            _content = content;
            _statusCode = statusCode;
        }

        public override void WriteContent(HttpListenerResponse response)
        {
            byte[] buf = Encoding.UTF8.GetBytes(_content);

            response.StatusCode = (int) _statusCode;
            response.ContentType = "text/plain";
            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }
    }
}
