using System;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Retrievers;

namespace CityWebServer.RequestHandlers
{
    public class MessageRequestHandler : RequestHandlerBase, ILogAppender
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

        public override Guid HandlerID
        {
            get { return new Guid("b4efeced-1dbb-435a-8999-9f8adaa5036e"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override String Name
        {
            get { return "Chirper Messages"; }
        }

        public override String Author
        {
            get { return "Rychard"; }
        }

        public override String MainPath
        {
            get { return "/Messages"; }
        }

        private readonly ChirpRetriever _chirpRetriever;

        public override Boolean ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Messages", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            // TODO: Customize request handling.
            var messages = _chirpRetriever.Messages;

            return JsonResponse(messages);
        }

        public MessageRequestHandler()
        {
            _chirpRetriever = new ChirpRetriever();
            _chirpRetriever.LogMessage += (sender, args) => { OnLogMessage(args.LogLine); };
        }
    }
}