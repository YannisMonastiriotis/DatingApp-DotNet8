using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker tracker) : Hub
    {
        
        public override async Task OnConnectedAsync()
        {

           var isOnline = await tracker.UserConnected(Context.User!.GetUsername(), Context.ConnectionId);
           
            if(isOnline) 
            {
                await  Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUsername());
            }

            var curentUsers = await tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", curentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
           var isOffline = await tracker.UserDisconnected(Context.User!.GetUsername(), Context.ConnectionId);

           if(isOffline)
           {
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUsername());
           }

            
            await base.OnDisconnectedAsync(exception);
        }
    }
}