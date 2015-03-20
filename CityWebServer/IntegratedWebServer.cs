using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using ColossalFramework;
using ColossalFramework.IO;
using ICities;

namespace CityWebServer
{
    public class IntegratedWebServer : ThreadingExtensionBase
    {
        private WebServer _server;
        private List<String> _logLines;
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
            this.InitializeServer();
            _requestHandlers = new List<IRequestHandler>();
            RegisterHandlers();

            base.OnCreated(threading);
        }

        private void InitializeServer()
        {
            if (_server != null)
            {
                _server.Stop();
                _server.LogMessage -= ServerOnLogMessage;
                _server = null;
            }

            LogMessage("Initializing Server...");

            // I'm not sure how I feel about making the port registration configurable.
            // Honestly, it sort of defeats the purpose, since other mods could potentially expect it to exist on a specific port.
            WebServer ws = new WebServer(HandleRequest, "http://localhost:8080/");
            _server = ws;
            _server.LogMessage += ServerOnLogMessage;
            _server.Run();
            LogMessage("Server Initialized.");
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

        #region Templates

        /// <summary>
        /// Gets the full path of the directory that contains this assembly.
        /// </summary>
        /// <remarks>
        /// This is currently rigged to work in a specific manner.
        /// </remarks>
        private String GetModPath()
        {
            // TODO: Find a better way of obtaining this information.
            String addonRoot = DataLocation.addonsPath;
            String modPath = Path.Combine(addonRoot, "Mods");
            String assemblyPath = Path.Combine(modPath, "CityWebServer_CityWebServer");
            return assemblyPath;
        }

        /// <summary>
        /// Gets the full content of a template.
        /// </summary>
        private String GetTemplate(String template)
        {
            // Templates seem like something we shouldn't handle internally.
            // Perhaps we should force request handlers to implement their own templating if they so desire, and maintain a more "API" approach within the core.
            String modPath = GetModPath();
            String templatePath = Path.Combine(modPath, "Templates");
            String specifiedTemplatePath = String.Format("{0}{1}{2}.html", templatePath, Path.DirectorySeparatorChar, template);

            if (File.Exists(specifiedTemplatePath))
            {
                String templateContents = File.ReadAllText(specifiedTemplatePath);
                return templateContents;
            }

            // All templates must at least have a #PAGEBODY# token.
            // If we can't find the specified template, just return a string that contains only that.
            return "#PAGEBODY#";
        }

        /// <summary>
        /// Retrieves the template with the specified name, and returns the contents of the template after replacing instances of the dictionary keys from <paramref name="tokenReplacements"/> with their coorresponding values.
        /// </summary>
        /// <param name="template">The name of the template to populate.</param>
        /// <param name="tokenReplacements">A dictionary containing key/value pairs for replacement.</param>
        /// <remarks>
        /// The value of <paramref name="template"/> should not include the file extension.
        /// </remarks>
        private String PopulateTemplate(String template, Dictionary<String, String> tokenReplacements)
        {
            try
            {
                String templateContents = GetTemplate(template);
                foreach (var tokenReplacement in tokenReplacements)
                {
                    templateContents = templateContents.Replace(tokenReplacement.Key, tokenReplacement.Value);
                }
                return templateContents;
            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
                return tokenReplacements["#PAGEBODY#"];
            }
        }

        /// <summary>
        /// Gets a dictionary that contains standard replacement tokens using the specified values.
        /// </summary>
        private Dictionary<String, String> GetTokenReplacements(String cityName, String title, String body)
        {
            // TODO: Order the request handlers by priority and name?  Whatever the decision is, there needs to be some defined order here.
            String nav = String.Join(Environment.NewLine, _requestHandlers.Select(obj => String.Format("<li><a href='{0}'>{1}</a></li>", obj.MainPath, obj.Name)).ToArray());

            return new Dictionary<string, string>
            {
                { "#PAGETITLE#", title },
                { "#NAV#", nav},
                { "#CSS#", ""}, // Moved directly into the template.
                { "#PAGEBODY#", body},
                { "#CITYNAME#", cityName},
                { "#JS#", ""}, // Moved directly into the template.
            };
        }

        #endregion Templates
       
        /// <summary>
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
            if (ServiceRoot(request, response)) { return; }
            if (ServiceLog(request, response)) { return; }

            // Get the request handler associated with the current request.
            var handler = _requestHandlers.FirstOrDefault(obj => obj.ShouldHandle(request));
            if (handler != null)
            {
                try
                {
                    handler.Handle(request, response);
                    return;
                }
                catch (Exception ex)
                {
                    String errorBody = String.Format("<h1>An error has occurred!</h1><pre>{0}</pre>", ex);
                    var tokens = GetTokenReplacements(_cityName, "Error", errorBody);
                    var template = PopulateTemplate("index", tokens);

                    byte[] buf = Encoding.UTF8.GetBytes(template);
                    response.ContentType = "text/html";
                    response.ContentLength64 = buf.Length;
                    response.OutputStream.Write(buf, 0, buf.Length);
                    return;
                }
            }

            // If the value is null, there must not be a request handler capable of servicing this request.
            const string defaultResponse = "<h1>No handlers exist to service this request.</h1>";
            var defaultTokens = GetTokenReplacements(_cityName, "Missing Handler", defaultResponse);
            var defaultTemplate = PopulateTemplate("index", defaultTokens);

            byte[] buf2 = Encoding.UTF8.GetBytes(defaultTemplate);
            response.ContentType = "text/html";
            response.ContentLength64 = buf2.Length;
            response.OutputStream.Write(buf2, 0, buf2.Length);
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain for class definitions that implement the <see cref="IRequestHandler"/> interface.  Those classes are instantiated and registered as request handlers.
        /// </summary>
        private void RegisterHandlers()
        {
            // TODO: This code doesn't detect handlers that are hot-loaded, if they're in a separate assembly.  We'll need to handle this somehow.

            var handlers = FindHandlers();
            foreach (var handler in handlers)
            {
                // Only register handlers that we don't already have an instance of.
                if (_requestHandlers.All(h => h.GetType() != handler))
                {
                    IRequestHandler handlerInstance = null;
                    Boolean exists = false;
                    try
                    {
                        handlerInstance = (IRequestHandler)Activator.CreateInstance(handler);
                            
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
                        // TODO: Add event handler for any handler that implements the ILogAppender interface.
                        _requestHandlers.Add(handlerInstance);
                        LogMessage(String.Format("Added Request Handler: {0}", handler.FullName));    
                    }
                }
            }
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain, and returns a collection of those that implement the <see cref="IRequestHandler"/> interface.
        /// </summary>
        private List<Type> FindHandlers()
        {
            List<Type> handlers = new List<Type>();
            var requestHandlerType = typeof (IRequestHandler);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyName = assembly.GetName().Name;

                // Skip any assemblies that we don't anticipate finding anything in.
                if (IgnoredAssemblies.Contains(assemblyName)) { continue; }

                int typeCount = 0;
                try
                {
                    var types = assembly.GetTypes().ToList();
                    typeCount = types.Count;
                    foreach (var type in types)
                    {
                        if (requestHandlerType.IsAssignableFrom(type) && type.IsClass)
                        {
                            handlers.Add(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(ex.ToString());
                }
                LogMessage(String.Format("Found {0} types in {1}, of which {2} were potential request handlers.", typeCount, assembly.GetName().Name, handlers.Count));
            }
            return handlers;
        }


        #region Reserved Endpoint Handlers

        /// <summary>
        /// Services requests to <c>~/</c>
        /// </summary>
        private Boolean ServiceRoot(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.Url.AbsolutePath.ToLower() == "/")
            {
                List<String> links = new List<string>();
                foreach (var requestHandler in this._requestHandlers.OrderBy(obj => obj.Priority))
                {
                    links.Add(String.Format("<li><a href='{1}'>{0}</a> by {2} (Priority: {3})</li>", requestHandler.Name, requestHandler.MainPath, requestHandler.Author, requestHandler.Priority));
                }

                String body = String.Format("<h1>Cities: Skylines - Integrated Web Server</h1><ul>{0}</ul>", String.Join("", links.ToArray()));
                var tokens = GetTokenReplacements(_cityName, "Home", body);
                var template = PopulateTemplate("index", tokens);

                byte[] buf = Encoding.UTF8.GetBytes(template);
                response.ContentType = "text/html";
                response.ContentLength64 = buf.Length;
                response.OutputStream.Write(buf, 0, buf.Length);

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
                    var tokens = GetTokenReplacements(_cityName, "Log", body);
                    var template = PopulateTemplate("index", tokens);

                    byte[] buf = Encoding.UTF8.GetBytes(template);
                    response.ContentType = "text/html";
                    response.ContentLength64 = buf.Length;
                    response.OutputStream.Write(buf, 0, buf.Length);

                    return true;
                }
            }
            return false;
        }

        #endregion
        
        #region Logging

        /// <summary>
        /// Adds a timestamp to the specified message, and appends it to the internal log.
        /// </summary>
        private void LogMessage(String message)
        {
            var dt = DateTime.Now;
            String line = String.Format("[{0} {1}] {2}{3}", dt.ToShortDateString(), dt.ToShortTimeString(), message, Environment.NewLine);
            _logLines.Add(line);
            //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, line);
        }

        /// <summary>
        /// Writes the value of <paramref name="args"/>.<see cref="LogAppenderEventArgs.LogLine"/> to the internal log.
        /// </summary>
        private void ServerOnLogMessage(object sender, LogAppenderEventArgs args)
        {
            LogMessage(args.LogLine);
        }

        #endregion
    }
}
