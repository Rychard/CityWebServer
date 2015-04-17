using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using CityWebServer.Extensibility;
using CityWebServer.Extensibility.Responses;
using CityWebServer.Helpers;
using CityWebServer.RequestHandlers;
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
        private String _cityName = "CityName";
        private string _wwwroot = null;
        private bool _secondPass = false;
        private bool _cwmPass = false;
        
        private Dictionary<string, CityWebMod> _cwMods; // the list of CityWebMods we've identified and registered
        private List<UserMod> _usrMods = null; // utility list of all UserMods found in ColossalFramework PluginManager

        /// <summary>
        /// Gets the root endpoint for which the server is configured to service HTTP requests.
        /// </summary>
        public static String Endpoint { get { return _endpoint; } }
        
        public List<String> LogLines { get { return _logLines; } }

        public String CityName { get { return _cityName; } }

        public String WebRoot { get { return _wwwroot; } }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegratedWebServer"/> class.
        /// </summary>
        public IntegratedWebServer()
        {
            // For the entire lifetime of this instance, we'll preseve log messages.
            // After a certain point, it might be worth truncating them, but we'll cross that bridge when we get to it.
            _logLines = new List<String>();
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
            if (null != _cwMods)
            {
                _cwMods.Clear();
                _cwMods = null;
            }
            if (null != _usrMods) // TODO: we ought to clean this up much sooner, only needed for init and i don't
                // like hanging on to these references for very long
            {
                _usrMods.Clear();
                _usrMods = null;
            }

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
        /// Performs init tasks on first request.
        /// </summary>
        /// <remarks>
        /// Certain plugin init processes must take place after all plugins have loaded. This defers these tasks
        /// until the first request arrives at the server, which should be after the plugin stack is done.
        /// </remarks>
        private void SecondPassInit()
        {
            LogMessage("Second pass initialization...");
            try
            {
                SimulationManager sm = Singleton<SimulationManager>.instance;
                if (null != sm)
                {
                    _cityName = sm.m_metaData.m_CityName;
                }
                else
                {
                    LogMessage(String.Format("failed to get city name: null SimulationManager"));
                    _cityName = "foo";
                }
            }
            catch (Exception ex)
            {
                LogMessage(String.Format("failed to get city name: {0}", ex));
                return;
            }
            LogMessage(String.Format("set city name: {0}", _cityName));
            
            try
            {
                _usrMods = UserMod.CollectPlugins();
                RegisterDefaultCWM();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                LogMessage(String.Format("failed to register plugins: {0}", ex));
                return;
            }

            _secondPass = true;
            LogMessage("Second pass complete.");
        }

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
            if (!_secondPass) SecondPassInit();
            if (!_cwmPass) RegisterCWM();

            // Get the request handler associated with the current request.
            string url = request.Url.AbsolutePath;
            string slug = null;
            String wwwroot = _wwwroot;
            bool handled = false;
            if (url.IsNullOrWhiteSpace() || url.Equals("/"))
            {
                slug = "root";
            }
            else if (url.StartsWith("/"))
            {
                string[] urlparts = url.Split('/');
                if (urlparts.Length < 3)
                {
                    slug = "root";
                }
                else
                {
                    slug = urlparts[1];
                }
            }

            if (slug != null && _cwMods.ContainsKey(slug))
            {
                CityWebMod cwm;
                _cwMods.TryGetValue(slug, out cwm);
                wwwroot = cwm.WebRoot;
                try
                {
                    handled = cwm.HandleRequest(request, response, slug, wwwroot);
                }
                catch (Exception ex)
                {
                    String errorBody = String.Format("<h1>An error has occurred!</h1><pre>{0}</pre>", ex);
                    var tokens = TemplateHelper.GetTokenReplacements(_cityName, "Error", this.Mods, errorBody);
                    var template = TemplateHelper.PopulateTemplate("content", _wwwroot, tokens);

                    IResponseFormatter errorResponseFormatter = new HtmlResponseFormatter(template);
                    errorResponseFormatter.WriteContent(response);

                    return;
                }
            }

            if (handled) return; // something handled it, great

            // At this point, we can guarantee that we don't need any game data, so we can safely start a new thread to perform the remaining tasks.
            if (ServiceFileRequest(wwwroot, request, response, slug)) return; // check for static files

            String body = String.Format("No resource is available at the specified filepath: {0}", url);
            IResponseFormatter notFoundResponseFormatter = new PlainTextResponseFormatter(body, HttpStatusCode.NotFound);
            notFoundResponseFormatter.WriteContent(response);
            return;
        }
        
        private static bool ServiceFileRequest(string wwwroot, HttpListenerRequest request, HttpListenerResponse response, string slug) {
            if (null == wwwroot || wwwroot.IsNullOrWhiteSpace()) return false;

            var relativePath = request.Url.AbsolutePath;
            if (!slug.Equals("root"))
            {
                relativePath = relativePath.Replace(String.Format("/{0}/", slug), "");
            }
            else
            {
                relativePath = relativePath.Substring(1);
            }
            relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            var absolutePath = Path.Combine(wwwroot, relativePath);
            if (!File.Exists(absolutePath)) return false;
            
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

            return true;
        }

        /// <summary>
        /// Searches all the plugins in the PluginManager for ones that implement <see cref="ICityWebMod"/>.
        /// </summary>
        private void RegisterCWM()
        {
            LogMessage("Looking for CityWebMods...");
            foreach (UserMod um in _usrMods) {
                if (!um.PluginInfo.isEnabled || um.PluginInfo.isBuiltin) continue;
                if (null == um.Mod) continue;

                LogMessage(String.Format("scanning {0} &lt;{1}&gt;...", um.PluginInfo.name, um.Mod.GetType()));
                CityWebMod cwm = CityWebMod.CreateByReflection(um, this);
                if (null == cwm) continue;

                string slug = cwm.ModID;
                if (slug == null || slug.IsNullOrWhiteSpace())
                {
                    LogMessage(String.Format("Invalid CityWebMod \"{0}\" by \"{1}\"", cwm.ModName, cwm.ModAuthor));
                }
                else if (_cwMods.ContainsKey(slug))
                {
                    LogMessage(String.Format("Conflicting CityWebMod; ID already exists! {0}:\"{1}\" by \"{2}\"", slug, cwm.ModName, cwm.ModAuthor));
                }
                else
                {
                    LogMessage(String.Format("Loaded CityWebMod {0}:\"{1}\" by \"{2}\"", slug, cwm.ModName, cwm.ModAuthor));
                    _cwMods.Add(slug, cwm);
                    List<IRequestHandler> hList = cwm.GetHandlers(this);
                    if (hList != null) {
                        foreach (IRequestHandler h in hList)
                        {
                            if (h is ILogAppender)
                            {
                                ILogAppender la = (h as ILogAppender);
                                la.LogMessage += RequestHandlerLogAppender_OnLogMessage;
                            }
                        }
                    }
                }
            }

            _cwmPass = true;
        }

        public ICityWebMod[] Mods { get { return _cwMods.Values.ToArray(); } }
       
        private void RequestHandlerLogAppender_OnLogMessage(object sender, LogAppenderEventArgs logAppenderEventArgs)
        {
            var senderTypeName = sender.GetType().Name;
            LogMessage(logAppenderEventArgs.LogLine, senderTypeName, false);
        }
        
        /// <summary>
        /// Does some initial setup dependent on scanning mods and also default/core CWM registration.
        /// </summary>
        private void RegisterDefaultCWM()
        {
            if (null != _cwMods)
            {
                _cwMods.Clear();
                _cwMods = null;
            }
            foreach (UserMod um in _usrMods) {
                if (!um.PluginInfo.isEnabled || um.PluginInfo.isBuiltin) continue;
                if (null == um.Mod) continue;
                if (um.Mod.Name.Equals("Integrated Web Server"))
                {
                    // hey it's me! get our web root
                    string testPath = Path.Combine(um.PluginInfo.modPath, "wwwroot");
                    if (Directory.Exists(testPath))
                    {
                        LogMessage(String.Format("Setting server wwwroot location: {0}", testPath));
                        _wwwroot = testPath;
                    }
                    break;
                }
            }

            LogMessage("adding default CityWebMods");
            _cwMods = new Dictionary<string, CityWebMod>();
            _cwMods.Add("root", new RootCWM(this));
            _cwMods.Add("log", new LogCWM(this));
            _cwMods.Add("api", new APICWM(this));
        }

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