using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using ColossalFramework.Plugins;
using CityWebServer.Extensibility;

namespace CityWebServer.Helpers
{
    public class CityWebMod : ICityWebMod
    {
        protected string _name = null;
        protected string _author = null;
        protected List<IRequestHandler> _handlers = null;
        protected string _wwwroot = null;
        protected string _ID = null;
        protected bool _isEnabled = false;
        protected bool _topMenu = true;

        public string ModName { get { return _name; } }
        public string ModAuthor { get { return _author; } }
        public string ModID { get { return _ID; } }
        public bool HasStatic { get { return _wwwroot != null; } }
        public bool IsEnabled { get { return _isEnabled; } }
        public bool TopMenu { get { return _topMenu; } }
        public string WebRoot { get { return _wwwroot; } }
        public List<IRequestHandler> GetHandlers(IWebServer server)
        {
            return _handlers;
        }
        
        protected CityWebMod()
        {
        }
        
        /*
        public CityWebMod(UserMod um, IWebServer server)
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
        */
        /*
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
         */

        public bool HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var handler = _handlers.FirstOrDefault(obj => obj.ShouldHandle(request, _ID));
            if (null == handler) return false;

            IResponseFormatter responseFormatterWriter = handler.Handle(request);
            responseFormatterWriter.WriteContent(response);
            return true;
        }

        public static CityWebMod CreateByReflection(UserMod um, IWebServer server)
        {
            // just casting um.Mod to ICityWebPlugin is not reliable; somtimes works, sometimes not
            // instead we need to reflect on it to identify mods that have implemented ICityWebMod
            Type ut = um.Mod.GetType();
            MethodInfo getHandlers = null;
            PropertyInfo pName = null;
            PropertyInfo pAuthor = null;
            PropertyInfo pID = null;
            PropertyInfo pTop = null;
            try
            {
                getHandlers = ut.GetMethod("GetHandlers");
                pName = ut.GetProperty("ModName");
                pAuthor = ut.GetProperty("ModAuthor");
                pID = ut.GetProperty("ModID");
                pTop = ut.GetProperty("TopMenu");
            }
            catch
            {
            }

            if (null != getHandlers && getHandlers.ReturnType != typeof(List<IRequestHandler>)) return null;
            if (null == getHandlers || null == pName || null == pAuthor || null == pID || null == pTop) return null;

            CityWebMod cwm = new CityWebMod();

            // grab values from the PluginManager.PluginInfo
            cwm._isEnabled = um.PluginInfo.isEnabled;
            string testPath = Path.Combine(um.PluginInfo.modPath, "wwwroot");
            if (Directory.Exists(testPath))
            {
                cwm._wwwroot = testPath;
            }
            
            // grab reflected property values
            cwm._name = (string)pName.GetValue(um.Mod, null);
            cwm._author = (string)pAuthor.GetValue(um.Mod, null);
            cwm._topMenu = (bool)pTop.GetValue(um.Mod, null);
            cwm._ID = (string)pID.GetValue(um.Mod, null);
            if (null != cwm._ID) cwm._ID = cwm._ID.ToLower().Replace(" ", "_");

            // invoke the method to get the list of request handlers managed by this CityWebMod
            List<IRequestHandler> h = null;
            try
            {
                h = (List<IRequestHandler>)getHandlers.Invoke(um.Mod, new Object[1] { server });
            }
            catch (Exception ex)
            {
                IntegratedWebServer.LogMessage(String.Format("failed to invoke GetHandlers: {0}", ex));
                return null;
            }
            if (null != h) {
                cwm._handlers = h;
            }
            else {
                cwm._handlers = new List<IRequestHandler>();
            }

            return cwm;
        }

    }
}
