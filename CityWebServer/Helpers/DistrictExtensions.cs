using System;
using CityWebServer.Models;

namespace CityWebServer.Helpers
{
    public static class DistrictExtensions
    {
        public static Boolean IsAlive(this District district)
        {
            // Get the flags on the district, to ensure we don't access garbage memory if it doesn't have a flag for District.Flags.Created
            Boolean alive = ((district.m_flags & District.Flags.Created) == District.Flags.Created);
            return alive;
        }

        public static PopulationGroup[] GetPopulation(this District district)
        {
            PopulationGroup[] ageGroups = 
            {
                new PopulationGroup("Children", district.GetChildrenCount()),
                new PopulationGroup("Teen", district.GetTeenCount()),
                new PopulationGroup("YoungAdult", district.GetYoungAdultCount()),
                new PopulationGroup("Adult", district.GetAdultCount()),
                new PopulationGroup("Senior", district.GetSeniorCount())
            };
            return ageGroups;
        }


        public static int GetChildrenCount(this District district)
        {
            return (int)district.m_childData.m_finalCount;
        }

        public static int GetTeenCount(this District district)
        {
            return (int)district.m_teenData.m_finalCount;
        }

        public static int GetYoungAdultCount(this District district)
        {
            return (int)district.m_youngData.m_finalCount;
        }

        public static int GetAdultCount(this District district)
        {
            return (int)district.m_adultData.m_finalCount;
        }

        public static int GetSeniorCount(this District district)
        {
            return (int)district.m_seniorData.m_finalCount;
        }
    }
}
