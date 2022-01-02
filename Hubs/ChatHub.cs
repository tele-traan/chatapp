using ChatApp.Util;
using ChatApp.Models;
using ChatApp.DB;

using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async override Task OnConnectedAsync()
        {   
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.User.Identity.Name;

            await Clients.All.SendAsync("MemberJoined", userName);

            var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            user.GlobalChatUser = new GlobalChatUser { UserName=userName, ConnectionId = Context.ConnectionId};
            dbContent.SaveChanges();

            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.User.Identity.Name;

            await Clients.All.SendAsync("MemberLeft", userName);
            var users = dbContent.GlobalChatUsers;
            var user = users.FirstOrDefault(u => u.UserName == userName);

            users.Remove(user);
            dbContent.SaveChanges();
            
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            string userName = Context.GetHttpContext().User.Identity.Name;
            var time = DateTime.Now.ToShortTimeString();
            await Clients.All.SendAsync("NewMessage", time, userName, message.Trim());
        }
    }
}