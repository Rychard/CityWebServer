using System;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;

namespace SampleWebServerExtension
{
    public class SampleHandler : IRequestHandler
    {
        public Guid HandlerID
        {
            get { return new Guid("1a255904-bf72-406e-b5e2-c5a43fdd9bba"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Sample"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Sample"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Sample", StringComparison.OrdinalIgnoreCase));
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            const String content = "This is a sample page!";

            byte[] buf = Encoding.UTF8.GetBytes(content);
            response.ContentType = "text/plain";
            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }
    }
}
