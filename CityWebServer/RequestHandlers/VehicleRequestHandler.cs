﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CityWebServer.Extensibility;
using ColossalFramework;
using JetBrains.Annotations;

namespace CityWebServer.RequestHandlers
{
    [UsedImplicitly]
    public class VehicleRequestHandler : RequestHandlerBase
    {
        public VehicleRequestHandler(IWebServer server)
            : base(server, "/Vehicle")
        {
        }

        public override IResponseFormatter Handle(HttpListenerRequest request)
        {
            var vehicleManager = Singleton<VehicleManager>.instance;

            if (request.Url.AbsolutePath.StartsWith("/Vehicle/List"))
            {
                List<ushort> vehicleIds = new List<ushort>();

                var len = vehicleManager.m_vehicles.m_buffer.Length;
                for (ushort i = 0; i < len; i++)
                {
                    if (vehicleManager.m_vehicles.m_buffer[i].m_flags == Vehicle.Flags.None) { continue; }

                    vehicleIds.Add(i);
                }

                return JsonResponse(vehicleIds);
            }

            List<ushort> s = new List<ushort>();

            foreach (var vehicle in vehicleManager.m_vehicles.m_buffer)
            {
                if (vehicle.m_flags == Vehicle.Flags.None) { continue; }

                if ((vehicle.m_flags & Vehicle.Flags.Spawned) == Vehicle.Flags.Spawned && (vehicle.m_flags & Vehicle.Flags.Created) == Vehicle.Flags.Created)
                {
                    var origin = (vehicle.m_sourceBuilding);
                    var target = (vehicle.m_targetBuilding);

                    if (origin > 0) { s.Add(origin); }
                    if (target > 0) { s.Add(target); }
                }
            }

            var grouped = s.GroupBy(obj => obj).Select(group => new { BuildingID = group.Key, Count = group.Count() }).OrderByDescending(obj => obj.Count).Select(obj => new { Building = BuildingManager.instance.GetBuildingName(obj.BuildingID, new InstanceID()), obj.Count }).ToList();

            return JsonResponse(grouped);
        }
    }
}