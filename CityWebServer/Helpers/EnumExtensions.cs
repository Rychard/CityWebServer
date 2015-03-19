using System;
using System.Collections.Generic;
using System.Linq;

namespace CityWebServer.Helpers
{
    public static class EnumHelper
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
