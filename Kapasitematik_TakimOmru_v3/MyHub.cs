using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace Kapasitematik_TakimOmru_v3
{
    [HubName("myHub")]
    public class MyHub : Hub
    {
        [HubMethodName("sendPiece")]
        public static void SendPiece()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.updateMessage();
        }

        public static void SendSubPiece()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.updatesubpiece();
        }
        public static void SendDetail()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.updatedetail();
        }
        public static void SendNotification()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.updatenotification();
        }
        public static void SendGridPiece()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.updategridpiece();
        }
        public static void SendGridDetay()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.griddetay();
        }
        public static void SendGridMachine()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.gridmachine();
        }
    }
}