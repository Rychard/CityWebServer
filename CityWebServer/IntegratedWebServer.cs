using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ApacheMimeTypes;
using CityWebServer.Extensibility;
using CityWebServer.Extensibility.Responses;
using CityWebServer.Helpers;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using JetBrains.Annotations;

namespace CityWebServer
{
    [UsedImplicitly]
    public class IntegratedWebServer : ThreadingExtensionBase, IWebServer
    {
        // Allows an arbitrary number of bindings by appending a number to the end.
        private const String WebServerHostKey = "webServerHost{0}";

        private static List<String> _logLines;
        private static string _endpoint;

        private WebServer _server;
        private List<IRequestHandler> _requestHandlers;
        private String _cityName = "CityName";

        // Not required, but prevents a number of spurious entries from making it to the log file.
        private static readonly List<String> IgnoredAssemblies = new List<String>
        {
            "Anonymously Hosted DynamicMethods Assembly",
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "Assembly-UnityScript-firstpass",
            "Boo.Lang",
            "ColossalManaged",
            "ICSharpCode.SharpZipLib",
            "ICities",
            "Mono.Security",
            "mscorlib",
            "System",
            "System.Configuration",
            "System.Core",
            "System.Xml",
            "UnityEngine",
            "UnityEngine.UI",
        };

        /// <summary>
        /// Gets the root endpoint for which the server is configured to service HTTP requests.
        /// </summary>
        public static String Endpoint
        {
            get { return _endpoint; }
        }

        /// <summary>
        /// Gets the full path to the directory where static pages are served from.
        /// </summary>
        public static String GetWebRoot()
        {
            var modPaths = PluginManager.instance.GetPluginsInfo().Select(obj => obj.modPath);
            foreach (var path in modPaths)
            {
                var testPath = Path.Combine(path, "wwwroot");

                if (Directory.Exists(testPath))
                {
                    return testPath;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an array containing all currently registered request handlers.
        /// </summary>
        public IRequestHandler[] RequestHandlers
        {
            get { return _requestHandlers.ToArray(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegratedWebServer"/> class.
        /// </summary>
        public IntegratedWebServer()
        {
            // For the entire lifetime of this instance, we'll preseve log messages.
            // After a certain point, it might be worth truncating them, but we'll cross that bridge when we get to it.
            _logLines = new List<String>();

            // We need a place to store all the request handlers that have been registered.
            _requestHandlers = new List<IRequestHandler>();
        }

        #region Create

        /// <summary>
        /// Called by the game after this instance is created.
        /// </summary>
        /// <param name="threading">The threading.</param>
        public override void OnCreated(IThreading threading)
        {
            InitializeServer();

            base.OnCreated(threading);
        }

        private void InitializeServer()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            LogMessage("Initializing Server...");

            List<String> bindings = new List<String>();

            int currentBinding = 1;
            String currentBindingKey = String.Format(WebServerHostKey, currentBinding);
            while (Configuration.HasSetting(currentBindingKey))
            {
                bindings.Add(Configuration.GetString(currentBindingKey));
                currentBinding++;
                currentBindingKey = String.Format(WebServerHostKey, currentBinding);
            }

            // If there are no bindings in the configuration file, we'll need to initialize those values.
            if (bindings.Count == 0)
            {
                const String defaultBinding = "http://localhost:8080/";
                bindings.Add(defaultBinding);

                // If there aren't any bindings, the value of currentBindingKey will never have made it past 1.
                // As a result, we can just use that.
                Configuration.SetString(currentBindingKey, defaultBinding);
                Configuration.SaveSettings();
            }

            // The endpoint used internally should always be the first binding in the configuration.
            // There's no need to use multiple bindings for internal references, we only need a single one.
            _endpoint = bindings.First();

            WebServer ws = new WebServer(HandleRequest, bindings.ToArray());
            _server = ws;
            _server.Run();
            LogMessage("Server Initialized.");

            _requestHandlers = new List<IRequestHandler>();

            try
            {
                RegisterHandlers();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        #endregion Create

        #region Release

        /// <summary>
        /// Called by the game before this instance is about to be destroyed.
        /// </summary>
        public override void OnReleased()
        {
            ReleaseServer();

            // TODO: Unregister from events (i.e. ILogAppender.LogMessage)
            _requestHandlers.Clear();

            Configuration.SaveSettings();

            base.OnReleased();
        }

        private void ReleaseServer()
        {
            LogMessage("Checking for existing server...");
            if (_server != null)
            {
                LogMessage("Server found; disposing...");
                _server.Stop();
                _server = null;
                LogMessage("Server Disposed.");
            }
        }

        #endregion Release

        /// <summary>;
        /// Handles the specified request.
        /// </summary>
        /// <remarks>
        /// Defers execution to an appropriate request handler, except for requests to the reserved endpoints: <c>~/</c> and <c>~/Log</c>.<br />
        /// Returns a default error message if an appropriate request handler can not be found.
        /// </remarks>
        private void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            LogMessage(String.Format("{0} {1}", request.HttpMethod, request.RawUrl));

            var simulationManager = Singleton<SimulationManager>.instance;
            _cityName = simulationManager.m_metaData.m_CityName;

            // There are two reserved endpoints: "/" and "/Log".
            // These take precedence over all other request handlers.
            if (ServiceRoot(request, response))
            {
                return;
            }

            if (ServiceLog(request, response))
            {
                return;
            }

            // Get the request handler associated with the current request.
            var handler = _requestHandlers.FirstOrDefault(obj => obj.ShouldHandle(request));
            if (handler != null)
            {
                try
                {
                    IResponseFormatter responseFormatterWriter = handler.Handle(request);
                    responseFormatterWriter.WriteContent(response);

                    return;
                }
                catch (Exception ex)
                {
                    String errorBody = String.Format("<h1>An error has occurred!</h1><pre>{0}</pre>", ex);
                    var tokens = TemplateHelper.GetTokenReplacements(_cityName, "Error", _requestHandlers, errorBody);
                    var template = TemplateHelper.PopulateTemplate("index", tokens);

                    IResponseFormatter errorResponseFormatter = new HtmlResponseFormatter(template);
                    errorResponseFormatter.WriteContent(response);

                    return;
                }
            }

            var wwwroot = GetWebRoot();

            // At this point, we can guarantee that we don't need any game data, so we can safely start a new thread to perform the remaining tasks.
            ServiceFileRequest(wwwroot, request, response);
        }

        private static void ServiceFileRequest(String wwwroot, HttpListenerRequest request, HttpListenerResponse response)
        {
            var relativePath = request.Url.AbsolutePath.Substring(1);
            relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            var absolutePath = Path.Combine(wwwroot, relativePath);

            if (File.Exists(absolutePath))
            {
                var extension = Path.GetExtension(absolutePath);
                response.ContentType = Apache.GetMime(extension);
                response.StatusCode = 200; // HTTP 200 - SUCCESS

                // Open file, read bytes into buffer and write them to the output stream.
                using (FileStream fileReader = File.OpenRead(absolutePath))
                {
                    byte[] buffer = new byte[4096];
                    int read;
                    while ((read = fileReader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        response.OutputStream.Write(buffer, 0, read);
                    }
                }
            }
            else
            {
                String body = String.Format("No resource is available at the specified filepath: {0}", absolutePath);

                IResponseFormatter notFoundResponseFormatter = new PlainTextResponseFormatter(body, HttpStatusCode.NotFound);
                notFoundResponseFormatter.WriteContent(response);
            }
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain for class definitions that implement the <see cref="IRequestHandler"/> interface.  Those classes are instantiated and registered as request handlers.
        /// </summary>
        private void RegisterHandlers()
        {
            IEnumerable<Type> handlers = FindHandlersInLoadedAssemblies();
            RegisterHandlers(handlers);
        }

        private void RegisterHandlers(IEnumerable<Type> handlers)
        {
            if (handlers == null) { return; }

            if (_requestHandlers == null)
            {
                _requestHandlers = new List<IRequestHandler>();
            }

            foreach (var handler in handlers)
            {
                // Only register handlers that we don't already have an instance of.
                if (_requestHandlers.Any(h => h.GetType() == handler))
                {
                    continue;
                }

                IRequestHandler handlerInstance = null;
                Boolean exists = false;

                try
                {
                    if (typeof(RequestHandlerBase).IsAssignableFrom(handler))
                    {
                        handlerInstance = (RequestHandlerBase)Activator.CreateInstance(handler, this);
                    }
                    else
                    {
                        handlerInstance = (IRequestHandler)Activator.CreateInstance(handler);
                    }

                    if (handlerInstance == null)
                    {
                        LogMessage(String.Format("Request Handler ({0}) could not be instantiated!", handler.Name));
                        continue;
                    }

                    // Duplicates handlers seem to pass the check above, so now we filter them based on their identifier values, which should work.
                    exists = _requestHandlers.Any(obj => obj.HandlerID == handlerInstance.HandlerID);
                }
                catch (Exception ex)
                {
                    LogMessage(ex.ToString());
                }

                if (exists)
                {
                    // TODO: Allow duplicate registrations to occur; previous registration is removed and replaced with a new one?
                    LogMessage(String.Format("Supressing duplicate handler registration for '{0}'", handler.Name));
                }
                else
                {
                    _requestHandlers.Add(handlerInstance);
                    if (handlerInstance is ILogAppender)
                    {
                        var logAppender = (handlerInstance as ILogAppender);
                        logAppender.LogMessage += RequestHandlerLogAppender_OnLogMessage;
                    }

                    LogMessage(String.Format("Added Request Handler: {0}", handler.FullName));
                }
            }
        }

        private void RequestHandlerLogAppender_OnLogMessage(object sender, LogAppenderEventArgs logAppenderEventArgs)
        {
            var senderTypeName = sender.GetType().Name;
            LogMessage(logAppenderEventArgs.LogLine, senderTypeName, false);
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain, and returns a collection of those that implement the <see cref="IRequestHandler"/> interface.
        /// </summary>
        private static IEnumerable<Type> FindHandlersInLoadedAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var handlers = FetchHandlers(assembly);
                foreach (var handler in handlers)
                {
                    yield return handler;
                }
            }
        }

        private static IEnumerable<Type> FetchHandlers(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;

            // Skip any assemblies that we don't anticipate finding anything in.
            if (IgnoredAssemblies.Contains(assemblyName)) { yield break; }

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

        #region Reserved Endpoint Handlers

        /// <summary>
        /// Services requests to <c>~/</c>
        /// </summary>
        private Boolean ServiceRoot(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.Url.AbsolutePath.ToLower() == "/")
            {
                List<String> links = new List<String>();
                foreach (var requestHandler in this._requestHandlers.OrderBy(obj => obj.Priority))
                {
                    links.Add(String.Format("<li><a href='{1}'>{0}</a> by {2} (Priority: {3})</li>", requestHandler.Name, requestHandler.MainPath, requestHandler.Author, requestHandler.Priority));
                }

                String body = String.Format("<h1>Cities: Skylines - Integrated Web Server</h1><ul>{0}</ul>", String.Join("", links.ToArray()));
                var tokens = TemplateHelper.GetTokenReplacements(_cityName, "Home", _requestHandlers, body);
                var template = TemplateHelper.PopulateTemplate("index", tokens);

                IResponseFormatter htmlResponseFormatter = new HtmlResponseFormatter(template);
                htmlResponseFormatter.WriteContent(response);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Services requests to <c>~/Log</c>
        /// </summary>
        private Boolean ServiceLog(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.Url.AbsolutePath.ToLower() == "/log")
            {
                {
                    String body = String.Format("<h1>Server Log</h1><pre>{0}</pre>", String.Join("", _logLines.ToArray()));
                    var tokens = TemplateHelper.GetTokenReplacements(_cityName, "Log", _requestHandlers, body);
                    var template = TemplateHelper.PopulateTemplate("index", tokens);

                    IResponseFormatter htmlResponseFormatter = new HtmlResponseFormatter(template);
                    htmlResponseFormatter.WriteContent(response);

                    return true;
                }
            }

            return false;
        }

        #endregion Reserved Endpoint Handlers

        #region Logging

        /// <summary>
        /// Adds a timestamp to the specified message, and appends it to the internal log.
        /// </summary>
        public static void LogMessage(String message, String label = null, Boolean showInDebugPanel = false)
        {
            var dt = DateTime.Now;
            String time = String.Format("{0} {1}", dt.ToShortDateString(), dt.ToShortTimeString());
            String messageWithLabel = String.IsNullOrEmpty(label) ? message : String.Format("{0}: {1}", label, message);
            String line = String.Format("[{0}] {1}{2}", time, messageWithLabel, Environment.NewLine);
            _logLines.Add(line);
            if (showInDebugPanel)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, line);
            }
        }

        /// <summary>
        /// Writes the value of <paramref name="args"/>.<see cref="LogAppenderEventArgs.LogLine"/> to the internal log.
        /// </summary>
        private void ServerOnLogMessage(object sender, LogAppenderEventArgs args)
        {
            LogMessage(args.LogLine);
        }

        #endregion Logging
    }
}