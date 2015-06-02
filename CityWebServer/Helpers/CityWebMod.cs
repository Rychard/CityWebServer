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
        protected String _name = null;
        protected String _author = null;
        protected List<IRequestHandler> _handlers = null;
        protected String _wwwroot = null;
        protected String _ID = null;
        protected Boolean _isEnabled = false;
        protected Boolean _topMenu = true;

        public String ModName { get { return _name; } }
        public String ModAuthor { get { return _author; } }
        public String ModID { get { return _ID; } }
        public Boolean HasStatic { get { return _wwwroot != null; } }
        public Boolean IsEnabled { get { return _isEnabled; } }
        public Boolean TopMenu { get { return _topMenu; } }
        public String WebRoot { get { return _wwwroot; } }
        public List<IRequestHandler> GetHandlers(IWebServer server)
        {
            return _handlers;
        }
        
        protected CityWebMod()
        {
        }        

        public Boolean HandleRequest(HttpListenerRequest request, HttpListenerResponse response, String slug, String wwwroot)
        {
            var handler = _handlers.FirstOrDefault(obj => obj.ShouldHandle(request, slug));
            if (handler == null) { return false; }

            IResponseFormatter responseFormatterWriter = handler.Handle(request, slug, wwwroot);
            responseFormatterWriter.WriteContent(response);
            return true;
        }

        public static CityWebMod CreateByReflection(UserMod um, IWebServer server)
        {
            // just casting um.Mod to ICityWebMod is not reliable; somtimes works, sometimes not
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

            if (getHandlers != null && getHandlers.ReturnType != typeof(List<IRequestHandler>)) { return null; }
            if (getHandlers == null || pName == null || pAuthor == null || pID == null || pTop == null) { return null; }

            CityWebMod cwm = new CityWebMod();

            // grab values from the PluginManager.PluginInfo
            cwm._isEnabled = um.PluginInfo.isEnabled;
            String testPath = Path.Combine(um.PluginInfo.modPath, "wwwroot");
            if (Directory.Exists(testPath))
            {
                cwm._wwwroot = testPath;
            }
            
            // grab reflected property values
            cwm._name = (String)pName.GetValue(um.Mod, null);
            cwm._author = (String)pAuthor.GetValue(um.Mod, null);
            cwm._topMenu = (Boolean)pTop.GetValue(um.Mod, null);
            cwm._ID = (String)pID.GetValue(um.Mod, null);
            if (cwm._ID != null)
            {
                cwm._ID = cwm._ID.ToLower().Replace(" ", "_");
            }

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
            if (h != null)
            {
                cwm._handlers = h;
            }
            else
            {
                cwm._handlers = new List<IRequestHandler>();
            }

            return cwm;
        }

    }
}
