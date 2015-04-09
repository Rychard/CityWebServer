using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace CityWebServer.Helpers
{
    // this is a wrapper class to encapsulate both the PluginManager.PluginInfo and its
    // associated IUserMod instance, to simplify spinning through these in tandem
    public class UserMod
    {
        private IUserMod _umod = null;
        private PluginManager.PluginInfo _pi = null;

        public IUserMod Mod { get { return _umod; } }
        public PluginManager.PluginInfo PluginInfo { get { return _pi; } }

        UserMod(PluginManager.PluginInfo pi)
        {
            _pi = pi;
            IUserMod[] insts = null;
            try
            {
                 insts = pi.GetInstances<IUserMod>();
            }
            catch
            {
            }
            if (null != insts && insts.Length > 0)
            {
                _umod = insts[0];
            }
        }

        /// <summary>
        /// Static factory to collect plugins from the PluginManager into a list of UserMods
        /// </summary>
        public static List<UserMod> CollectPlugins()
        {
            List<UserMod> miList = new List<UserMod>();
            PluginManager pm = Singleton<PluginManager>.instance;
            if (null == pm) return miList;

            List<PluginManager.PluginInfo> piList = new List<PluginManager.PluginInfo>(pm.GetPluginsInfo());
            foreach (PluginManager.PluginInfo pi in piList)
            {
                miList.Add(new UserMod(pi));
            }
            
            return miList;
        }

    }
}
