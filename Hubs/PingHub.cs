using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System;
using ChatApp.Util;
using ChatApp.DB;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Hubs
{
    public class PingHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.Session.GetString("UserName");
            var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            user.ConnectionId = Context.ConnectionId;
            user.OnlineNow = true;
            dbContent.SaveChanges();
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.Session.GetString("UserName");
            var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            user.OnlineNow = false;
            user.ConnectionId = null;
            dbContent.SaveChanges();
            await base.OnDisconnectedAsync(exception);
        }
    }
}