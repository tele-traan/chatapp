using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ChatApp.DB;
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

            var dbContent = httpContext.GetDbContent();
            var users = dbContent.GlobalChatUsers;
            var user = users.FirstOrDefault(u => u.UserName == userName);
            users.Remove(user);
            dbContent.SaveChanges();
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessageRequest(string message, string sender)
        {
            var time = DateTime.Now.ToShortTimeString();
            await Clients.All.SendAsync("NewMessage", time, sender, message.Trim());
        }
        /*public async Task MemberLeft(string whoLeft)
        {
            HttpContext httpContext = GetHttpContextExtensions.GetHttpContext(Context);
            DBContent dbContent =  httpContext.RequestServices.GetRequiredService<DBContent>();
            var user = dbContent.RoomUsers.FirstOrDefault(u => u.UserName == whoLeft);
            dbContent.RoomUsers.Remove(user);
            dbContent.SaveChanges();
            await Clients.All.SendAsync("MemberLeft", whoLeft);
        }*/
    }
}