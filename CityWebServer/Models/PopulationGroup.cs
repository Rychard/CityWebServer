using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityWebServer.Models
{
    public class PopulationGroup
    {
        public String Name { get; set; }

        public int Amount { get; set; }

        public PopulationGroup()
        {
        }

        public PopulationGroup(String name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
