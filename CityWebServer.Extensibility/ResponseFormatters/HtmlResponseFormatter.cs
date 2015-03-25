using System;
using System.Net;
using System.Text;

namespace CityWebServer.Extensibility.Responses
{
    internal class HtmlResponseFormatter : IResponseFormatter
    {
        private readonly String _content;
        private readonly HttpStatusCode _statusCode;

        public HtmlResponseFormatter(String content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _content = content;
            _statusCode = statusCode;
        }

        public override void WriteContent(HttpListenerResponse response)
        {
            byte[] buf = Encoding.UTF8.GetBytes(_content);

            response.StatusCode = (int) _statusCode;
            response.ContentType = "text/html";
            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }
    }
}
