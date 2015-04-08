using System;
using System.Collections.Generic;
using System.Net;
using CityWebServer.Extensibility;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class BuildingRequestHandler : RequestHandlerBase
    {
        public BuildingRequestHandler(IWebServer server)
            : base(server, "/Building")
        {
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