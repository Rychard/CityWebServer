using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class BuildingRequestHandler : RequestHandlerBase
    {
        public override Guid HandlerID
        {
            get { return new Guid("03897cb0-d53f-4189-a613-e7d22705dc2f"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override String Name
        {
            get { return "Vehicle"; }
        }

        public override String Author
        {
            get { return "Rychard"; }
        }

        public override String MainPath
        {
            get { return "/Building"; }
        }

        public override Boolean ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.StartsWith("/Building", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            var buildingManager = Singleton<BuildingManager>.instance;

            if (request.Url.AbsolutePath.StartsWith("/Building/List"))
            {
                List<ushort> buildingIDs = new List<ushort>();

                var len = buildingManager.m_buildings.m_buffer.Length;
                for (ushort i = 0; i < len; i++)
                {
                    if (buildingManager.m_buildings.m_buffer[i].m_flags == Building.Flags.None) { continue; }

                    buildingIDs.Add(i);
                }

                return JsonResponse(buildingIDs);
            }

            foreach (var building in buildingManager.m_buildings.m_buffer)
            {
                if (building.m_flags == Building.Flags.None) { continue; }

                // TODO: Something with Buildings.
            }

            return JsonResponse("");
        }
    }
}
