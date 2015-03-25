using System;
using System.Net;
using CityWebServer.Extensibility;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class BudgetRequestHandler : BaseHandler
    {
        public override Guid HandlerID
        {
            get { return new Guid("87205a0d-1b53-47bd-91fa-9cddf0a3bd9e"); }
        }

        public override int Priority
        {
            get { return 100; }
        }

        public override String Name
        {
            get { return "Budget"; }
        }

        public override String Author
        {
            get { return "Rychard"; }
        }

        public override String MainPath
        {
            get { return "/Budget"; }
        }

        public override Boolean ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Budget", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponse Handle(HttpListenerRequest request)
        {
            // TODO: Expand upon this to expose substantially more information.
            var economyManager = Singleton<EconomyManager>.instance;
            long income;
            long expenses;
            economyManager.GetIncomeAndExpenses(new ItemClass(), out income, out expenses);

            Decimal formattedIncome = Math.Round(((Decimal)income / 100), 2);
            Decimal formattedExpenses = Math.Round(((Decimal)expenses / 100), 2);

            var content = String.Format("Income: {0:C}{2}Expenses: {1:C}", formattedIncome, formattedExpenses, Environment.NewLine);

            return HtmlResponse(content);
        }
    }
}