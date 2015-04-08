using System;
using System.Net;

namespace CityWebServer.Extensibility
{
    public abstract class RestfulRequestHandlerBase : RequestHandlerBase
    {
        public RestfulRequestHandlerBase(IWebServer server, String mainPath)
            : base(server, mainPath)
        {
        }

        public override string MainPath { get { return _mainPath; } }

        public override bool ShouldHandle(HttpListenerRequest request, String slug)
        {
            return (request.Url.AbsolutePath.StartsWith(String.Format("/{0}{1}", slug, _mainPath), StringComparison.OrdinalIgnoreCase));
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            switch (request.HttpMethod)
            {
                case "GET":
                    return HandleGetRequest(request);

                case "POST":
                    return HandlePostRequest(request);

                case "PUT":
                    return HandlePutRequest(request);

                case "DELETE":
                    return HandleDeleteRequest(request);

                default:
                    return JsonResponse("400 Bad Request", HttpStatusCode.BadRequest);
            }
        }

        protected virtual IResponseFormatter HandleGetRequest(HttpListenerRequest request)
        {
            return JsonResponse("405 Method Not Allowed", HttpStatusCode.MethodNotAllowed);
        }

        protected virtual IResponseFormatter HandlePostRequest(HttpListenerRequest request)
        {
            return JsonResponse("405 Method Not Allowed", HttpStatusCode.MethodNotAllowed);
        }

        protected virtual IResponseFormatter HandlePutRequest(HttpListenerRequest request)
        {
            return JsonResponse("405 Method Not Allowed", HttpStatusCode.MethodNotAllowed);
        }

        protected virtual IResponseFormatter HandleDeleteRequest(HttpListenerRequest request)
        {
            return JsonResponse("405 Method Not Allowed", HttpStatusCode.MethodNotAllowed);
        }
    }
}