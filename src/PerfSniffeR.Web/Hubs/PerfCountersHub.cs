using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PerfSniffeR.Web.Infrastructure;

namespace PerfSniffeR.Web.Hubs
{
    [HubName("perfMonHub")]
    public class PerfCountersHub : Hub
    {
        public dynamic AvailableCounters()
        {
            return PerfMonitor.ServiceCounters.Select(counter =>
                new
                {
                    FullName = string.Format(@"{0}\{1}\{2}\{3}", counter.MachineName, counter.CategoryName, counter.CounterName, counter.InstanceName),
                    MachineName = counter.MachineName,
                    CategoryName = counter.CategoryName,
                    CounterName = counter.CounterName,
                    InstanceName = counter.InstanceName
                });
        }
    }
}