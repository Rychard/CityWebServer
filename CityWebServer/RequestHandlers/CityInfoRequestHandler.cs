using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;
using CityWebServer.Models;
using ColossalFramework;

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

        private Dictionary<String, Boolean> GetPolicies()
        {
            var policies = EnumHelper.GetValues<DistrictPolicies.Policies>();
            Dictionary<String, Boolean> policyValues = new Dictionary<String, Boolean>();
            var districtManager = Singleton<DistrictManager>.instance;

            foreach (var policy in policies)
            {
                String policyName = Enum.GetName(typeof (DistrictPolicies.Policies), policy);
                Boolean isEnabled = districtManager.IsCityPolicySet(DistrictPolicies.Policies.AlligatorBan);
                policyValues.Add(policyName, isEnabled);
            }
            return policyValues;
        }

        private DistrictInfo GetDistrictInfo(int districtID)
        {
            var districtManager = Singleton<DistrictManager>.instance;
            var district = GetDistrict(districtID);

            if (!IsValid(district)) { return null; }

            String districtName = String.Empty;

            if (districtID == 0)
            {
                // The district with ID 0 is always the global district.  
                // It receives an auto-generated name by default, but the game always displays the city name instead.
                districtName = "City";
            }
            else
            {
                districtName = districtManager.GetDistrictName(districtID);   
            }

            var model = new DistrictInfo
            {
                DistrictID = districtID,
                DistrictName = districtName,
                TotalPopulationCount = (int)district.m_populationData.m_finalCount,
                PopulationData = GetPopulationGroups(districtID),
                CurrentHouseholds = (int)district.m_residentialData.m_finalAliveCount,
                AvailableHouseholds = (int)district.m_residentialData.m_finalHomeOrWorkCount,
                CurrentJobs = (int)district.m_commercialData.m_finalAliveCount + (int)district.m_industrialData.m_finalAliveCount + (int)district.m_officeData.m_finalAliveCount + (int)district.m_playerData.m_finalAliveCount,
                AvailableJobs = (int)district.m_commercialData.m_finalHomeOrWorkCount + (int)district.m_industrialData.m_finalHomeOrWorkCount + (int)district.m_officeData.m_finalHomeOrWorkCount + (int)district.m_playerData.m_finalHomeOrWorkCount,
                AverageLandValue = district.GetLandValue(),
                WeeklyTouristVisits = (int)district.m_tourist1Data.m_averageCount + (int)district.m_tourist2Data.m_averageCount + (int)district.m_tourist3Data.m_averageCount
            };
            return model;
        }

        private District GetDistrict(int? districtID = null)
        {
            if (districtID == null) { districtID = 0; }
            var districtManager = Singleton<DistrictManager>.instance;
            var district = districtManager.m_districts.m_buffer[districtID.Value];
            return district;
        }

        public Boolean IsValid(District district)
        {
            return (district.m_flags != District.Flags.None);
        }

        private PopulationGroup[] GetPopulationGroups(int? districtID = null)
        {
            var district = GetDistrict(districtID);
            return district.GetPopulation();
        }

        private IEnumerable<int> GetDistricts()
        {
            var districtManager = Singleton<DistrictManager>.instance;

            // This is the value used in Assembly-CSharp, so I presume that's the maximum number of districts allowed.
            const int count = 128; 

            var districts = districtManager.m_districts.m_buffer;

            for (int i = 0; i < count; i++)
            {
                if (!districts[i].IsAlive()) { continue; }
                yield return i;
            }
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
            var districtIDs = GetDistricts().ToArray();
            response.WriteJson(districtIDs);
        }

        private void HandleDistrict(HttpListenerRequest request, HttpListenerResponse response)
        {
            var districtIDs = GetDistrictsFromRequest(request);

            List<DistrictInfo> districtInfoList = new List<DistrictInfo>();
            foreach (var districtID in districtIDs)
            {
                var districtInfo = this.GetDistrictInfo(districtID);
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
                districtIDs = GetDistricts();
            }
            return districtIDs;
        }
    }
}
