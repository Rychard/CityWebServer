using System;
using System.Xml.Serialization;

namespace CityWebServer.Models
{
    public class CityInfo
    {
        public String Name { get; set; }
        public DateTime Time { get; set; }
        public DistrictInfo GlobalDistrict { get; set; }
        public DistrictInfo[] Districts { get; set; }
    }
}