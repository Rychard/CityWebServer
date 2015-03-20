using System;
using System.Net;
using System.Text;
using CityWebServer.Extensibility;
using ColossalFramework;

namespace CityWebServer.RequestHandlers
{
    public class BudgetRequestHandler : IRequestHandler, ILogAppender
    {
        public event EventHandler<LogAppenderEventArgs> LogMessage;

        private void OnLogMessage(String message)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogAppenderEventArgs(message));
            }
        }

        public Guid HandlerID
        {
            get { return new Guid("87205a0d-1b53-47bd-91fa-9cddf0a3bd9e"); }
        }

        public int Priority
        {
            get { return 100; }
        }

        public string Name
        {
            get { return "Budget"; }
        }

        public string Author
        {
            get { return "Rychard"; }
        }

        public string MainPath
        {
            get { return "/Budget"; }
        }

        public bool ShouldHandle(HttpListenerRequest request)
        {
            return (request.Url.AbsolutePath.Equals("/Budget", StringComparison.OrdinalIgnoreCase));
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            // TODO: Expand upon this to expose substantially more information.
            var economyManager = Singleton<EconomyManager>.instance;
            long income;
            long expenses;
            economyManager.GetIncomeAndExpenses(new ItemClass(), out income, out expenses);

            Decimal formattedIncome = Math.Round(((Decimal)income / 100), 2);
            Decimal formattedExpenses = Math.Round(((Decimal)expenses/ 100), 2);

            var content = String.Format("Income: {0:C}{2}Expenses: {1:C}", formattedIncome, formattedExpenses, Environment.NewLine);

            byte[] buf = Encoding.UTF8.GetBytes(content);
            response.ContentType = "text/html";
            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }

        public BudgetRequestHandler()
        {
        }
    }
}