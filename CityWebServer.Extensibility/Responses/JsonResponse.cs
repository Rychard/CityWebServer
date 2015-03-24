using System.Net;
using System.Text;

namespace CityWebServer.Extensibility.Responses
{
    internal class JsonResponse<T> : IResponse
    {
        private readonly T _content;
        private readonly HttpStatusCode _statusCode;

        public JsonResponse(T content, HttpStatusCode statusCode)
        {
            _content = content;
            _statusCode = statusCode;
        }

        public override void WriteContent(HttpListenerResponse response)
        {
            var writer = new JsonFx.Json.JsonWriter();
            var serializedData = writer.Write(_content);

            byte[] buf = Encoding.UTF8.GetBytes(serializedData);

            response.StatusCode = (int) _statusCode;
            response.ContentType = "text/json";
            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }
    }
}
