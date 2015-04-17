using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Reflection;
using CityWebServer.Helpers;
using CityWebServer.Extensibility;
using CityWebServer.Extensibility.Responses;

namespace CityWebServer.RequestHandlers
{
    class HandlerCWM : CityWebMod
    {
        public HandlerCWM(IWebServer server)
        {
            _ID = "handlers";
            _name = "Naked Handlers";
            _author = "nezroy";
            _isEnabled = true;
            _wwwroot = server.WebRoot;
            _topMenu = true;

            _handlers = new List<IRequestHandler>();
            _handlers.Add(new HandlersRequestHandler(server, this));
        }

        private class HandlersRequestHandler : RequestHandlerBase
        {
            private HandlerCWM _container;
            public HandlersRequestHandler(IWebServer server, HandlerCWM container)
                : base(server, "/")
            {
                _container = container;
            }

            public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
            {
                List<String> links = new List<String>();
                foreach (var h in _container.GetHandlers(_server).OrderBy(obj => obj.Priority))
                {
                    if (h.MainPath.Equals("/")) { continue; }
                    links.Add(String.Format("<li><a href='{0}'>[{3}] {1}</a> by {2}</li>", h.MainPath, h.Name, h.Author, h.Priority));
                }

                String body = String.Format("<ul>{0}</ul>", String.Join("", links.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_server.CityName, "Naked Handlers", _server.Mods, body);
                var template = TemplateHelper.PopulateTemplate("content", wwwroot, tokens);

                return new HtmlResponseFormatter(template);
            }
        }

        public void AddHandlers(UserMod umod, IWebServer server)
        {
            Assembly modAssm = umod.Mod.GetType().Assembly;
            var handlers = FetchHandlers(modAssm);
            foreach (var handler in handlers)
            {
                IRequestHandler handlerInstance = null;
                try
                {
                    if (typeof(RequestHandlerBase).IsAssignableFrom(handler))
                    {
                        handlerInstance = (RequestHandlerBase)Activator.CreateInstance(handler, server);
                    }
                    else
                    {
                        handlerInstance = (IRequestHandler)Activator.CreateInstance(handler);
                    }

                    if (handlerInstance == null)
                    {
                        IntegratedWebServer.LogMessage(String.Format("Request Handler ({0}) could not be instantiated!", handler.Name));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    IntegratedWebServer.LogMessage(ex.ToString());
                }

                AddHandler(handlerInstance);
                if (handlerInstance is ILogAppender)
                {
                    var logAppender = (handlerInstance as ILogAppender);
                    logAppender.LogMessage += RequestHandlerLogAppender_OnLogMessage;
                }

                IntegratedWebServer.LogMessage(String.Format("added request handler: {0}", handler.FullName));
            }
        }

        private static IEnumerable<Type> FetchHandlers(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;

            Type[] types = new Type[0];
            try
            {
                types = assembly.GetTypes();
            }
            catch { }

            foreach (var type in types)
            {
                Boolean isValid = false;
                try
                {
                    isValid = typeof(IRequestHandler).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
                }
                catch { }

                if (isValid)
                {
                    yield return type;
                }
            }
        }

        public void AddHandler(IRequestHandler handler)
        {
            // TODO: use prio to put it in correct order
            // int prio = handler.Priority;
            _handlers.Add(handler);
        }
        
        // TODO: this is copypasta from IWS; find a cleaner way to do this
        private void RequestHandlerLogAppender_OnLogMessage(object sender, LogAppenderEventArgs logAppenderEventArgs)
        {
            var senderTypeName = sender.GetType().Name;
            IntegratedWebServer.LogMessage(logAppenderEventArgs.LogLine, senderTypeName, false);
        }
    }
}
