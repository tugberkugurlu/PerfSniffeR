using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace PerfSniffeR.Web.Hubs
{
    [HubName("perfMonHub")]
    public class PerfCountersHub : Hub
    {
    }
}