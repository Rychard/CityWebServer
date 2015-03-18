using System;
using System.Net;
using System.Text;
using System.Threading;
using CityWebServer.Extensibility;

namespace CityWebServer
{
    public class WebServer : ILogAppender
    {
        public event EventHandler<LogAppenderEventArgs> LogMessage;
        private void OnLogMessage(String message)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogAppenderEventArgs(message));
            }
        }

        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported) { throw new NotSupportedException("This wouldn't happen if you upgraded your operating system more than once a decade."); }

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0) { throw new ArgumentException("prefixes"); }

            // A responder method is required
            if (method == null) { throw new ArgumentException("method"); }

            foreach (string s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes) : this(prefixes, method) 
        {
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(RequestHandlerCallback, _listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    OnLogMessage(ex.ToString());
                }
            });
        }

        private void RequestHandlerCallback(Object context)
        {
            var ctx = context as HttpListenerContext;
            try
            {
                if (ctx != null)
                {
                    string rstr = _responderMethod(ctx.Request);
                    byte[] buf = Encoding.UTF8.GetBytes(rstr);
                    ctx.Response.ContentLength64 = buf.Length;
                    ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                }
            }
            catch {} // Suppress any exceptions.
            finally
            {
                if (ctx != null)
                {
                    // Ensure that the stream is never left open.
                    ctx.Response.OutputStream.Close();
                }
            }
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
