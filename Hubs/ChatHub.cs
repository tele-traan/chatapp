using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using ChatApp.DB;
using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            var usersRepo = this.GetService<IUsersRepository>();

            string userName = Context.GetHttpContext().User.Identity.Name;

            await Clients.All.SendAsync("MemberJoined", userName);

            var user = usersRepo.GetUser(userName);
            user.GlobalChatUser = new GlobalChatUser { UserName=userName, ConnectionId = Context.ConnectionId};
            usersRepo.UpdateUser(user);

            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var usersRepo = this.GetService<IUsersRepository>();
            string userName = Context.GetHttpContext().User.Identity.Name;

            await Clients.All.SendAsync("MemberLeft", userName);

            var user = usersRepo.GetUser(userName);
            user.GlobalChatUser = null;
            usersRepo.UpdateUser(user);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            var gcRepo = this.GetService<IGCRepository>();
            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = this.GetService<IGCUsersRepository>().GetUser(userName);
            if (user.ConnectionId != Context.ConnectionId) Context.Abort();
            var time = DateTime.Now;
            gcRepo.AddMessage(new() { Text = message, SenderName = userName, DateTime = time });
            await Clients.All.SendAsync("NewMessage", time.ToShortTimeString(), userName, message.Trim());
        }
    }
}