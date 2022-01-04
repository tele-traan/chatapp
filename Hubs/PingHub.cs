using System.Threading.Tasks;
using System.Linq;
using System;

using ChatApp.Util;
using ChatApp.DB;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class PingHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.User.Identity.Name;
            var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);
            user.OnlineNow = true;
            user.ConnectionId = Context.ConnectionId;
            dbContent.SaveChanges();
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == httpContext.User.Identity.Name);
            //while (user == null)
            //{
            //    user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == httpContext.User.Identity.Name);
            //}
            user.OnlineNow = false;
            user.ConnectionId = null;
            dbContent.SaveChanges();
            await base.OnDisconnectedAsync(exception);
        }
    }
}