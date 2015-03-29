using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ColossalFramework.IO;
using JsonFx.Serialization;

namespace CityWebServer.Helpers
{
    public static class Configuration
    {
        private static readonly Object LockerObject = new Object();
        private static readonly String _filePath;
        private static List<Setting> _settings;

        static Configuration()
        {
            _filePath = GetSettingsFilePath();
            LoadSettings();
        }

        public static String GetSettingsFilePath()
        {
            var localApplicationDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var vendorRoot = System.IO.Path.Combine(localApplicationDataRoot, "Colossal Order");
            var appRoot = System.IO.Path.Combine(vendorRoot, "Cities_Skylines");
            var filePath = System.IO.Path.Combine(appRoot, "ModSettings.json");
            
            // This works for Windows, but not for OSX.
            if (CanAccess(filePath))
            {
                return filePath;
            }

            // If we just use a filename, it will exist in the root directory of the game's files.  Not ideal, but it'll work.
            return "ModSettings.json";
        }

        private static Boolean CanAccess(String filePath)
        {
            try
            {
                using (var fileStream = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void LoadSettings()
        {
            lock (LockerObject)
            {
                using (var fileStream = System.IO.File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    using (TextReader tr = new StreamReader(fileStream))
                    {
                        var jsonReader = new JsonFx.Json.JsonReader();
                        var deserialized = jsonReader.Read(tr, typeof(List<Setting>));
                        var settings = deserialized as List<Setting>;
                        _settings = settings ?? new List<Setting>();
                    }
                }
            }
        }

        public static void SaveSettings()
        {
            lock (LockerObject)
            {
                // No settings to save?  Don't save anything.
                if (_settings == null) { return; }

                var dataWriterSettings = new DataWriterSettings
                {
                    PrettyPrint = true,
                };

                if (System.IO.File.Exists(_filePath))
                {
                    System.IO.File.Delete(_filePath);
                }
                using (var fileStream = System.IO.File.Open(_filePath, FileMode.CreateNew, FileAccess.Write))
                {
                    using (TextWriter tw = new StreamWriter(fileStream))
                    {
                        var jsonWriter = new JsonFx.Json.JsonWriter(dataWriterSettings);
                        jsonWriter.Write(_settings, tw);
                    }
                }
            }
        }

        private static String GetSettingRaw(String key)
        {
            lock (LockerObject)
            {
                String raw;
                var matches = _settings.Where(obj => obj.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (matches.Any())
                {
                    raw = matches.First().Value;
                }
                else
                {
                    raw = null;
                }
                return raw;
            }
        }

        private static void SetSettingRaw(String key, String value, String type)
        {
            lock (LockerObject)
            {
                var matches = _settings.Where(obj => obj.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (matches.Any())
                {
                    matches.First().Value = value;
                }
                else
                {
                    _settings.Add(new Setting
                    {
                        Key = key,
                        Value = value,
                        Type = type
                    });
                }
            }
        }

        public static Boolean HasSetting(String key)
        {
            lock (LockerObject)
            {
                if (_settings == null) { throw new Exception("Settings aren't loaded!"); }
                var matches = _settings.Where(obj => obj.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                return (matches.Any());
            }
        }

        public static Type GetSettingType(String key)
        {
            lock (LockerObject)
            {
                var matches = _settings.Where(obj => obj.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (matches.Any())
                {
                    var t = matches.First().Type;
                    switch (t)
                    {
                        case "string":
                            return typeof(string);

                        case "int":
                            return typeof(int);

                        case "float":
                            return typeof(float);

                        case "double":
                            return typeof(double);

                        default:
                            return typeof(object);
                    }
                }
                return null;
            }
        }

        #region String

        public static String GetString(String key)
        {
            var raw = GetSettingRaw(key);
            return raw;
        }

        public static void SetString(String key, String value)
        {
            SetSettingRaw(key, value, "string");
        }

        #endregion String

        #region Integer

        public static int GetInt(String key)
        {
            var raw = GetSettingRaw(key);
            int i;
            if (int.TryParse(raw, out i))
            {
                return i;
            }
            return default(int);
        }

        public static void SetInt(String key, int value)
        {
            SetSettingRaw(key, value.ToString(CultureInfo.InvariantCulture), "int");
        }

        #endregion Integer

        #region Float

        public static float GetFloat(String key)
        {
            var raw = GetSettingRaw(key);
            float f;
            if (float.TryParse(raw, out f))
            {
                return f;
            }
            return default(float);
        }

        public static void SetFloat(String key, float value)
        {
            SetSettingRaw(key, value.ToString(CultureInfo.InvariantCulture), "float");
        }

        #endregion Float

        #region Double

        public static double GetDouble(String key)
        {
            var raw = GetSettingRaw(key);
            double d;
            if (double.TryParse(raw, out d))
            {
                return d;
            }
            return default(double);
        }

        public static void SetDouble(String key, double value)
        {
            SetSettingRaw(key, value.ToString(CultureInfo.InvariantCulture), "double");
        }

        #endregion Double
    }

    public class Setting
    {
        public String Key { get; set; }

        public String Value { get; set; }

        public String Type { get; set; }
    }
}