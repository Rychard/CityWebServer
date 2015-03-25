using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class TransportRequestHandler : BaseHandler
    {
        public override Guid HandlerID
        {
            get { return new Guid("89c8ef27-fc8c-4fe8-9793-1f6432feb179"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override String Name
        {
            get { return "Transport"; }
        }

        public override String Author
        {
            get { return "Rychard"; }
        }

        public override String MainPath
        {
            get { return "/Transport"; }
        }

        public override Boolean ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Transport", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
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
