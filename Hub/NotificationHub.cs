﻿using Microsoft.AspNet.SignalR;

namespace Hub
{
    public class NotificationHub : Microsoft.AspNet.SignalR.Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

        public void Hello()
        {
            Clients.All.hello();
        }

        public static void SayHello()
        {
            hubContext.Clients.All.hello();
        }
    }
}
