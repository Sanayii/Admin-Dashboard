using Microsoft.AspNetCore.SignalR;

namespace Admin_Dashboard.Models
{
    public class DashboardHub : Hub
    {
        public async Task UpdateStats(int admins, int customers, int craftsmen)
        {
            await Clients.All.SendAsync("ReceiveStatsUpdate", admins, customers, craftsmen);
        }

        public async Task RequestStatsUpdate()
        {
            await Clients.Caller.SendAsync("ReceiveStatsUpdate", 0, 0, 0);
        }

        public async Task DashboardUpdated(DashboardViewModel model)
        {
            await Clients.All.SendAsync("UpdateDashboard", model);
        }
    }
}