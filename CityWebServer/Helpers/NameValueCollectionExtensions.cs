using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace CityWebServer.Helpers
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Determines whether the specified key exists in the current collection.
        /// </summary>
        /// <returns>Returns <c>true</c> if the specified key exists, otherwise <c>false</c></returns>
        public static Boolean HasKey(this NameValueCollection nvc, String key)
        {
            return nvc.AllKeys.Any(obj => obj == key);
        }

        /// <summary>
        /// Gets the value of the specified key as an integer.
        /// </summary>
        /// <returns>Returns the value of the integer in the specified key, or <c>null</c> if the value is not a valid integer.</returns>
        public static int? GetInteger(this NameValueCollection nvc, String key)
        {
            var value = nvc.Get(key);
            int result;
            if (Int32.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }
    }
}
