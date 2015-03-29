using System;

namespace CityWebServer.Models
{
    public class PublicTransportLine
    {
        public String Name { get; set; }

        public int VehicleCount { get; set; }

        public int StopCount { get; set; }

        public PopulationGroup[] Passengers { get; set; }
    }
}