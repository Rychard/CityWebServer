using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using ColossalFramework.Plugins;
using CityWebServer.Extensibility;

namespace CityWebServer.Helpers
{
    // This gets encapsulated because I don't want to maintain a reference to either the plugininfo or the plugin instance
    // to remove any chance for these references to interfere with the engine's plugin loading/processing chain.
    // Plus most of what we need can be figured out once up front anyway. This also allows us to fake entries for core
    // things not actually loaded by plugins too.
    public class CityWebPluginInfo : IPluginInfo
    {
        protected string _name = null;
        protected string _author = null;
        protected List<IRequestHandler> _handlers = null;
        protected string _wwwroot = null;
        protected string _ID = null;
        protected bool _isEnabled = false;
        protected bool _topMenu = true;

        public string Name { get { return _name; } }
        public string PluginName { get { return _name; } }
        public string Author { get { return _author; } }
        public string PluginAuthor { get { return _author; } }
        public string ID { get { return _ID; } }
        public string PluginID { get { return _ID; } }
        public bool HasStatic { get { return _wwwroot != null; } }
        public bool IsEnabled { get { return _isEnabled; } }
        public bool TopMenu { get { return _topMenu; } }
        public string WebRoot { get { return _wwwroot; } }
        public List<IRequestHandler> Handlers { get { return _handlers; } }
        
        protected CityWebPluginInfo()
        {
        }
        
        public CityWebPluginInfo(ICityWebPlugin p, PluginManager.PluginInfo pi, IWebServer server)
        {
            _ID = p.PluginID.ToLower().Replace(" ", "_");
            _name = p.PluginName;
            _author = p.PluginAuthor;
            _topMenu = p.TopMenu;
            _isEnabled = pi.isEnabled;
            if (_isEnabled)
            {
                string testPath = Path.Combine(pi.modPath, "wwwroot");
                if (Directory.Exists(testPath))
                {
                    _wwwroot = testPath;
                }
                List<IRequestHandler> h = p.GetHandlers(server);
                if (null != h)
                {
                    _handlers = h;
                }
                else
                {
                    _handlers = new List<IRequestHandler>();
                }
            }
        }

        public CityWebPluginInfo(string name, string author, string ID, IEnumerable<IRequestHandler> h)
        {
            _ID = ID;
            _name = name;
            _author = author;
            _isEnabled = true;
            if (null != h)
            {
                _handlers = new List<IRequestHandler>(h);
            }
            else
            {
                _handlers = new List<IRequestHandler>();
            }
        }

        public bool HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var handler = _handlers.FirstOrDefault(obj => obj.ShouldHandle(request, _ID));
            if (null == handler) return false;

            IResponseFormatter responseFormatterWriter = handler.Handle(request);
            responseFormatterWriter.WriteContent(response);
            return true;
        }

    }
}
