using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;
using CityWebServer.Models;

namespace CityWebServer.RequestHandlers
{
    public class CityInfoRequestHandler : IRequestHandler
    {
        public Guid HandlerID
        {
            get { return new Guid("eeada0d0-f1d2-43b0-9595-2a6a4d917631"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "City Info"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/CityInfo"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/CityInfo", StringComparison.OrdinalIgnoreCase));
        }

        private void GetMaxLevels()
        {
            int maxCommercialHigh = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.CommercialHigh);
            int maxCommercialLow = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.CommercialLow);
            int maxDistant = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.Distant);
            int maxIndustrial = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.Industrial);
            int maxNone = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.None);
            int maxOffice = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.Office);
            int maxResidentialHigh = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.ResidentialHigh);
            int maxResidentialLow = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.ResidentialLow);
            int maxUnzoned = ZonedBuildingWorldInfoPanel.GetMaxLevel(ItemClass.Zone.Unzoned);
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.QueryString.HasKey("showList"))
            {
                HandleDistrictList(response);
                return;
            }

            HandleDistrict(request, response);
        }

        private void HandleDistrictList(HttpListenerResponse response)
        {
            var districtIDs = DistrictInfo.GetDistricts().ToArray();
            response.WriteJson(districtIDs);
        }

        private void HandleDistrict(HttpListenerRequest request, HttpListenerResponse response)
        {
            var districtIDs = GetDistrictsFromRequest(request);

            List<DistrictInfo> districtInfoList = new List<DistrictInfo>();
            foreach (var districtID in districtIDs)
            {
                var districtInfo = DistrictInfo.GetDistrictInfo(districtID);
                districtInfoList.Add(districtInfo);
            }

            var cityInfo = new CityInfo
            {
                Districts = districtInfoList.ToArray(),
            };

            response.WriteJson(cityInfo);
        }

        private IEnumerable<int> GetDistrictsFromRequest(HttpListenerRequest request)
        {
            IEnumerable<int> districtIDs;
            if (request.QueryString.HasKey("districtID"))
            {
                List<int> districtIDList = new List<int>();
                var districtID = request.QueryString.GetInteger("districtID");
                if (districtID.HasValue)
                {
                    districtIDList.Add(districtID.Value);
                }
                districtIDs = districtIDList;
            }
            else
            {
                districtIDs = DistrictInfo.GetDistricts();
            }
            return districtIDs;
        }
    }
}