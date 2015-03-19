using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using CityWebServer.Helpers;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class OverviewRequestHandler : IRequestHandler
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
            get { return "Overview"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Overview"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Overview", StringComparison.OrdinalIgnoreCase));
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

        private Dictionary<String, int> GetPopulationCount(int? districtID = null)
        {
            if (districtID == null) { districtID = 0; }

            var districtManager = Singleton<DistrictManager>.instance;
            var district = districtManager.m_districts.m_buffer[districtID.Value];

            Dictionary<String, int> ageGroups = new Dictionary<String, int>();
            ageGroups.Add("Children", (int) district.m_childData.m_finalCount);
            ageGroups.Add("Teen", (int)district.m_teenData.m_finalCount);
            ageGroups.Add("YoungAdult", (int)district.m_youngData.m_finalCount);
            ageGroups.Add("Adult", (int)district.m_adultData.m_finalCount);
            ageGroups.Add("Senior", (int)district.m_seniorData.m_finalCount);
            return ageGroups;
        }

        private void asdf()
        {
            var district = Singleton<DistrictManager>.instance.m_districts.m_buffer[0];
            int total = (int)district.m_populationData.m_finalCount;
            int num6 = (int)district.m_residentialData.m_finalAliveCount;
            int num7 = (int)district.m_residentialData.m_finalHomeOrWorkCount;
            int num8 = (int)district.m_commercialData.m_finalAliveCount + (int)district.m_industrialData.m_finalAliveCount + (int)district.m_officeData.m_finalAliveCount + (int)district.m_playerData.m_finalAliveCount;
            int num9 = (int)district.m_commercialData.m_finalHomeOrWorkCount + (int)district.m_industrialData.m_finalHomeOrWorkCount + (int)district.m_officeData.m_finalHomeOrWorkCount + (int)district.m_playerData.m_finalHomeOrWorkCount;
            int num10 = (int)district.m_tourist1Data.m_averageCount + (int)district.m_tourist2Data.m_averageCount + (int)district.m_tourist3Data.m_averageCount;
        }

        public string Handle(HttpListenerRequest request)
        {
            var citizenManager = Singleton<CitizenManager>.instance;

            

            var citizens = citizenManager.m_instances;

            foreach (var citizen in citizens.m_buffer)
            {
                citizen.
                citizen.m_flags * Citizen.Flags.
            }

            // TODO: Expand upon this to expose substantially more information.
            var economyManager = Singleton<EconomyManager>.instance;
            long income;
            long expenses;
            economyManager.GetIncomeAndExpenses(new ItemClass(), out income, out expenses);

            Decimal formattedIncome = Math.Round(((Decimal)income / 100), 2);
            Decimal formattedExpenses = Math.Round(((Decimal)expenses/ 100), 2);

            return String.Format("Income: {0:C}{2}Expenses: {1:C}", formattedIncome, formattedExpenses, Environment.NewLine);
        }
    }
}
