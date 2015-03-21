using System;
using System.Xml.Serialization;

namespace CityWebServer.Models
{
    public class CityInfo
    {
        public DistrictInfo[] Districts { get; set; }
    }

    public class DistrictInfo
    {
        public int DistrictID { get; set; }
        public String DistrictName { get; set; }
        public PopulationGroup[] PopulationData { get; set; }
        public int TotalPopulationCount { get; set; }
        public int CurrentHouseholds { get; set; }
        public int AvailableHouseholds { get; set; }
        public int CurrentJobs { get; set; }
        public int AvailableJobs { get; set; }
        public int WeeklyTouristVisits { get; set; }
        public int AverageLandValue { get; set; }
    }

    public class PopulationGroup
    {
        [XmlAttribute("Name")]
        public String Name { get; set; }

        [XmlAttribute("Amount")]
        public int Amount { get; set; }

        public PopulationGroup() { }

        public PopulationGroup(String name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }

}
