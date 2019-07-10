using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class CheckinHub : Hub
    {
        public static void Refresh()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<CheckinHub>();
            hubContext.Clients.All.refreshPage();
        }
    }
}