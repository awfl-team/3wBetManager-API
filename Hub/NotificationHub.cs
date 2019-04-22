using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Models;

namespace Hub
{
    [HubName("notificationHub")]
    public class NotificationHub : Microsoft.AspNet.SignalR.Hub
    {
        private static readonly List<UserHub> ConnectedUsers =
            new List<UserHub>();

        [HubMethodName("notification")]
        public void SendNotification(string sentTo, string message)
        {
            var user = ConnectedUsers.Find(u => u.Username == sentTo);
            if (user == null) return;
            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            context.Clients.Client(user.ConnectionId).NotifyUser(message);
        }

        public override Task OnConnected()
        {
            var connectionId = Context.ConnectionId;
            var userName = Context.QueryString["username"];
            var user = new UserHub{Username = userName, ConnectionId = connectionId};
            var index = ConnectedUsers.FindIndex(u => u.ConnectionId == connectionId);
            if (index == -1)
            {
                ConnectedUsers.Add(user);
                return base.OnConnected();
            }

            ConnectedUsers[index] = user;
            Console.WriteLine(userName);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            var connectionId = Context.ConnectionId;
            var index = ConnectedUsers.FindIndex(u => u.ConnectionId == connectionId);
            ConnectedUsers.RemoveAt(index);

            return base.OnDisconnected(stopCalled);
        }
    }
}