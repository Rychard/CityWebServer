using System;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Retrievers;
using JetBrains.Annotations;

namespace CityWebServer.RequestHandlers
{
    [UsedImplicitly]
    public class MessageRequestHandler : RequestHandlerBase
    {
        private readonly ChirpRetriever _chirpRetriever;

        public MessageRequestHandler(IWebServer server) : base(server, new Guid("b4efeced-1dbb-435a-8999-9f8adaa5036e"), "Chirper Messages", "Rychard", 100, "/Messages")
        {
            _chirpRetriever = new ChirpRetriever();
            _chirpRetriever.LogMessage += (sender, args) => { OnLogMessage(args.LogLine); };
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            // TODO: Customize request handling.
            var messages = _chirpRetriever.Messages;

            return JsonResponse(messages);
        }
    }
}