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

        public MessageRequestHandler(IWebServer server)
            : base(server, "/Messages")
        {
            _chirpRetriever = new ChirpRetriever();
            _chirpRetriever.LogMessage += (sender, args) => { OnLogMessage(args.LogLine); };
        }

        public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
        {
            // TODO: Customize request handling.
            var messages = _chirpRetriever.Messages;

            return JsonResponse(messages);
        }
    }
}