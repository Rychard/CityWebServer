using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;
using CityWebServer.Models;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class TransportRequestHandler : IRequestHandler
    {
        public Guid HandlerID
        {
            get { return new Guid("89c8ef27-fc8c-4fe8-9793-1f6432feb179"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Transport"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Transport"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Transport", StringComparison.OrdinalIgnoreCase));
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            var transportManager = Singleton<TransportManager>.instance;

            var lines = transportManager.m_lines.m_buffer;
            List<PublicTransportLine> lineModels = new List<PublicTransportLine>();

            foreach (var line in lines)
            {
                if (line.m_flags == TransportLine.Flags.None) { continue; }

                var passengers = line.m_passengers;
                List<PopulationGroup> passengerGroups = new List<PopulationGroup>
                {
                    new PopulationGroup("Child", (int) passengers.m_childPassengers.m_finalCount),
                    new PopulationGroup("Teen", (int) passengers.m_teenPassengers.m_finalCount),
                    new PopulationGroup("Young Adult", (int) passengers.m_youngPassengers.m_finalCount),
                    new PopulationGroup("Adult", (int) passengers.m_adultPassengers.m_finalCount),
                    new PopulationGroup("Senior", (int) passengers.m_seniorPassengers.m_finalCount),
                    new PopulationGroup("Tourist", (int) passengers.m_touristPassengers.m_finalCount),
                    new PopulationGroup("Resident", (int) passengers.m_residentPassengers.m_finalCount),
                    new PopulationGroup("Car-Owning", (int) passengers.m_carOwningPassengers.m_finalCount)
                };

                var stops = line.CountStops(0); // The parameter is never used.
                var vehicles = line.CountVehicles(0); // The parameter is never used.
                
                var lineModel = new PublicTransportLine
                {
                    Name = String.Format("{0} {1}", line.Info.name, (int)line.m_lineNumber),
                    StopCount = stops,
                    VehicleCount = vehicles,
                    Passengers = passengerGroups.ToArray(),
                };
                lineModels.Add(lineModel);
            }

            lineModels = lineModels.OrderBy(obj => obj.Name).ToList();

            response.WriteJson(lineModels);
        }
    }

    public class PublicTransportLine
    {
        public String Name { get; set; }
        public int VehicleCount { get; set; }
        public int StopCount { get; set; }

        public PopulationGroup[] Passengers { get; set; }
    }
}
