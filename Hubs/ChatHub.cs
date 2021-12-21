using Microsoft.AspNetCore.SignalR;
using ChatApp.Util;
using ChatApp.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            ISession session = httpContext.Session;
            string userName = session.GetString("UserName");

            await Clients.All.SendAsync("MemberJoined", userName);

            var dbContent = httpContext.GetDbContent();
            var users = dbContent.GlobalChatUsers;
            users.Add(new GlobalChatUser { UserName=userName});
            dbContent.SaveChanges();

            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            ISession session = httpContext.Session;
            string userName = session.GetString("UserName");

            await Clients.All.SendAsync("MemberLeft", userName);
            var dbContent = httpContext.GetDbContent();
            var users = dbContent.GlobalChatUsers;
            var user = users.FirstOrDefault(u => u.UserName == userName);

            users.Remove(user);
            dbContent.SaveChanges();
            
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            ISession session = Context.GetHttpContext().Session;
            string userName = session.GetString("UserName");
            var time = DateTime.Now.ToShortTimeString();
            await Clients.All.SendAsync("NewMessage", time, userName, message.Trim());
        }
    }
}