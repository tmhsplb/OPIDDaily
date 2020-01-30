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

        // Based on:
        //  https://www.codeproject.com/articles/1124691/signalr-progress-bar-simple-example-sending-live-d
        public static void SendProgress(string progressMessage, int progressCount, int totalItems)
        {
            //IN ORDER TO INVOKE SIGNALR FUNCTIONALITY DIRECTLY FROM SERVER SIDE WE MUST USE THIS
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<DailyHub>();

            //CALCULATING PERCENTAGE BASED ON THE PARAMETERS SENT
            var percentage = (progressCount * 100) / totalItems;

            //PUSHING DATA TO ALL CLIENTS
            hubContext.Clients.All.AddProgress(progressMessage, percentage + "%");
        }
    }
}