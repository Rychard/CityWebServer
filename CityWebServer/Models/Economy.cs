using System;

namespace CityWebServer.Models
{
    public class Economy
    {
        public IncomeExpense[] IncomesAndExpenses { get; set; }

        public TaxRate[] TaxRates { get; set; }
    }

    public class TaxRate
    {
        public String GroupName { get; set; }

        public int Rate { get; set; }

        // Tax Rate: Low-Density Residential
        // Tax Rate: High-Density Residential
        // Tax Rate: Low-Density Commercial
        // Tax Rate: High-Density Commercial
        // Tax Rate: Industry
        // Tax Rate: Offices
    }

    public class IncomeExpense
    {
        public String Group { get; set; }

        public string SubGroup { get; set; }

        public Double Amount { get; set; }

        // Tax Income: Low-Density Residential
        // Tax Income: High-Density Residential
        // Tax Income: Low-Density Commercial
        // Tax Income: High-Density Commercial
        // Tax Income: Industry
        // Tax Income: Offices

        // Income: Citizens
        // Income: Tourists

        // Income: Bus/Train/Metro?

        // Upkeep Expense: Roads
        // Upkeep Expense: Electricity
        // Upkeep Expense: Water
        // Upkeep Expense: Garbage
        // Upkeep Expense: Unique Buildings
        // Upkeep Expense: Healthcare
        // Upkeep Expense: Education
        // Upkeep Expense: Police
        // Upkeep Expense: Firefighters
        // Upkeep Expense: Parks
        // Upkeep Expense: Bus/Train/Metro?
        // Upkeep Expense: Taxes???
        // Upkeep Expense: Policy
    }
}