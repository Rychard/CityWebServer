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

    public class PopulationGroup
    {
        [XmlAttribute("Name")]
        public String Name { get; set; }

        [XmlAttribute("Amount")]
        public int Amount { get; set; }

        public PopulationGroup()
        {
        }

        public PopulationGroup(String name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}