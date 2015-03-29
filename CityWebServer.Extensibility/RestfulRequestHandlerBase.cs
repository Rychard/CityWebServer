using System;
using System.Net;

namespace CityWebServer.Extensibility
{
    public abstract class RestfulRequestHandlerBase : RequestHandlerBase
    {
        protected Guid _handlerID;
        protected String _name;
        protected String _author;
        protected int _priority;
        protected String _mainPath;

        public RestfulRequestHandlerBase(Guid handlerID, String name, String author, int priority, String mainPath)
            : base()
        {
            _handlerID = handlerID;
            _name = name;
            _author = author;
            _priority = priority;
            _mainPath = mainPath;
        }

        public override Guid HandlerID { get { return _handlerID; } }

        public override int Priority { get { return _priority; } }

        public override string Name { get { return _name; } }

        public override string Author { get { return _author; } }

        public override string MainPath { get { return _mainPath; } }

        public override bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.StartsWith(_mainPath, StringComparison.OrdinalIgnoreCase));
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