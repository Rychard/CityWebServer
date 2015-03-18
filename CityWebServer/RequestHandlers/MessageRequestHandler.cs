using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using CityWebServer.Retrievers;

namespace CityWebServer.RequestHandlers
{
    public class MessageRequestHandler : IRequestHandler, ILogAppender
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

        public Guid HandlerID
        {
            get { return new Guid("b4efeced-1dbb-435a-8999-9f8adaa5036e"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Chirper Messages"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Messages"; }
        }

        private readonly ChirpRetriever _chirpRetriever;

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Messages", StringComparison.OrdinalIgnoreCase));
        }

        public string Handle(HttpListenerRequest request)
        {
            // TODO: Customize request handling.
            XmlSerializer serializer = new XmlSerializer(typeof(ChirperMessage[]));
            var messages = _chirpRetriever.Messages;
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, messages);
            return "<pre>" + sw.ToString() + "</pre>";
        }

        public MessageRequestHandler()
        {
            _chirpRetriever = new ChirpRetriever();
            _chirpRetriever.LogMessage += (sender, args) => { OnLogMessage(args.LogLine); };
        }
    }
}
