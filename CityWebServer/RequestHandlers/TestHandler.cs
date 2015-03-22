using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using UnityEngine;

namespace CityWebServer.RequestHandlers
{
    public class TestHandler : IRequestHandler, ILogAppender
    {
        public event EventHandler<LogAppenderEventArgs> LogMessage;

        private void OnLogMessage(String message)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogAppenderEventArgs(message));
            }
        }

        public Guid HandlerID
        {
            get { return new Guid("e7019de3-ac6c-4099-a384-a1de325b4a9d"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Test"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Test"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Test", StringComparison.OrdinalIgnoreCase));
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            List<String> s = new List<string>();

            var uis = GameObject.FindObjectsOfType<UIComponent>();

            foreach (var ui in uis)
            {
                s.Add(ui.name);
            }

            s.Sort();
            s = s.Distinct().ToList();



            //Dump();

            
            //s.Add(TemplateHelper.GetModPath());
            //var plugins = PluginManager.instance.GetPluginsInfo();

            //foreach (var pluginInfo in plugins)
            //{
            //    s.Add(pluginInfo.modPath);
            //}

            response.WriteJson(s);

            //String content = DateTime.Now.ToFileTime().ToString();

            //byte[] buf = Encoding.UTF8.GetBytes(content);
            //response.ContentType = "text/plain";
            //response.ContentLength64 = buf.Length;
            //response.OutputStream.Write(buf, 0, buf.Length);
        }

        void Dump()
        {
            System.Collections.Generic.List<UITextureAtlas.SpriteInfo> spritelist = UIView.GetAView().defaultAtlas.sprites;
            foreach (UITextureAtlas.SpriteInfo sprite in spritelist)
            {
                try
                {
                    byte[] pngbytes = sprite.texture.EncodeToPNG();
                    String filename = this.MakeValidFileName(sprite.name);
                    System.IO.File.WriteAllBytes("D:\\sprites\\" + filename + ".png", pngbytes);
                }
                catch (Exception ex)
                {
                }
            }
        }

        // http://stackoverflow.com/a/847251
        public string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}