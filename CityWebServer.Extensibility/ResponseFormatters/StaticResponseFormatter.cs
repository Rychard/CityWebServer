using System;
using System.Net;
using System.Linq;
using System.Text;
using System.IO;
using ColossalFramework.Plugins;

namespace CityWebServer.Extensibility.Responses
{
    internal class StaticResponseFormatter : IResponseFormatter
    {
        private readonly String _wwwroot;
        private readonly HttpListenerRequest _request;

        public StaticResponseFormatter(String wwwroot, HttpListenerRequest request)
        {
            _wwwroot = wwwroot;
            _request = request;
        }

        public override void WriteContent(HttpListenerResponse response)
        {
            var relativePath = _request.Url.AbsolutePath.Substring(1);
            relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            var absolutePath = Path.Combine(_wwwroot, relativePath);

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
        /// Gets the full path to the directory where static pages are served from.
        /// Requires UNIQUE directory names across all mods.
        /// </summary>
        public static String GetWebRoot(String myRootPath)
        {
            var modPaths = PluginManager.instance.GetPluginsInfo().Select(obj => obj.modPath);
            foreach (var path in modPaths)
            {
                var testPath = Path.Combine(path, myRootPath);

                if (Directory.Exists(testPath))
                {
                    return testPath;
                }
            }
            return null;
        }

    }
}
