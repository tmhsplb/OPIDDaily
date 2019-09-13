using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class DailyHub : Hub
    {
        public static void Refresh()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<DailyHub>();
            hubContext.Clients.All.refreshPage();
        }

        public static void RefreshClient(int nowServing)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<DailyHub>();
            hubContext.Clients.All.refreshClientPage(nowServing);
        }

        public static void GetClientHistory(int nowServing)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<DailyHub>();
            hubContext.Clients.All.getClientHistory(nowServing);
        }
    }
}