using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CityWebServer.Extensibility;

namespace CityWebServer.Helpers
{
    public class CityWebPlugin : ICityWebPlugin
    {
        private string _name = null;
        private string _author = null;
        private string _ID = null;
        private bool _topMenu = false;
        List<IRequestHandler> _handlers = null;

        public string PluginName { get { return _name; } }
        public string PluginAuthor { get { return _author; } }
        public string PluginID { get { return _ID; } }
        public bool TopMenu { get { return _topMenu; } }
        
        public List<IRequestHandler> GetHandlers(IWebServer server)
        {
            return _handlers;
        }

        public static CityWebPlugin CreateByReflection(UserMod um, IWebServer server)
        {
            // just casting um.Mod to ICityWebPlugin is not reliable; somtimes works, sometimes not
            // instead we reflect through methods to identify valid plugins
            Type ut = um.Mod.GetType();
            MethodInfo[] mia = ut.GetMethods();
            bool isCityWebPlugin = false;
            for (int i = 0; i < mia.Length; i++)
            {
                MethodInfo mi = mia[i];
                if (mi.Name.Equals("GetHandlers") && mi.ReturnType == typeof(List<IRequestHandler>))
                {
                    isCityWebPlugin = true;
                    break;
                }
            }

            if (!isCityWebPlugin) return null;

            CityWebPlugin cwp = new CityWebPlugin();

            // since casting doesn't work reliably, we need to fill out a placeholder class with source
            // values by reflection as well
            PropertyInfo[] pia = ut.GetProperties();
            for (int i = 0; i < pia.Length; i++)
            {
                PropertyInfo pi = pia[i];
                if (pi.Name.Equals("PluginName")) cwp._name = (string)pi.GetValue(um.Mod, null);
                else if (pi.Name.Equals("PluginAuthor")) cwp._author = (string)pi.GetValue(um.Mod, null);
                else if (pi.Name.Equals("PluginID")) cwp._ID = (string)pi.GetValue(um.Mod, null);
                else if (pi.Name.Equals("TopMenu")) cwp._topMenu = (bool)pi.GetValue(um.Mod, null);
            }

            for (int i = 0; i < mia.Length; i++)
            {
                MethodInfo mi = mia[i];
                if (mi.Name.Equals("GetHandlers") && mi.ReturnType == typeof(List<IRequestHandler>))
                {
                    cwp._handlers = (List<IRequestHandler>)mi.Invoke(um.Mod, new Object[1]{ server });
                }
            }

            return cwp;
        }
    }
}
