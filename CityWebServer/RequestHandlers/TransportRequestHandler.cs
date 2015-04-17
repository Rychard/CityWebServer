using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using ColossalFramework;
using JetBrains.Annotations;

namespace CityWebServer.RequestHandlers
{
    [UsedImplicitly]
    public class TransportRequestHandler : RequestHandlerBase
    {
        public TransportRequestHandler(IWebServer server)
            : base(server, "/Transport")
        {
        }

        public override IResponseFormatter Handle(HttpListenerRequest request, String slug, String wwwroot)
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

            return JsonResponse(lineModels);
        }
    }
}