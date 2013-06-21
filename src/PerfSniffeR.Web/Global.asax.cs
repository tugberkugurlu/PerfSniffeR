using System;
using System.Threading.Tasks;
using System.Web.Routing;
using PerfSniffeR.Web.Infrastructure;

namespace PerfSniffeR.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.MapHubs();
            PerfMonitor perfMon = new PerfMonitor(1000);
            Task.Factory.StartNew(async () => await perfMon.MonitorAsync());
        }
    }
}