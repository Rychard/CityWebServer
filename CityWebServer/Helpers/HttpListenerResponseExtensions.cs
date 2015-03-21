using System.Net;
using System.Text;

namespace CityWebServer.Helpers
{
    public static class HttpListenerResponseExtensions
    {
        /// <summary>
        /// Serializes the object to a JSON string, and writes it to the current stream.
        /// </summary>
        /// <remarks>If the object cannot be serialized, an exception is thrown.</remarks>
        public static void WriteJson<T>(this HttpListenerResponse response, T obj)
        {
            var writer = new JsonFx.Json.JsonWriter();
            var serializedData = writer.Write(obj);

            byte[] buf = Encoding.UTF8.GetBytes(serializedData);
            response.ContentType = "text/json";
            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }

    }
}
