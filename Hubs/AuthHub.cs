using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using ChatApp.Models;
using ChatApp.Util;
using ChatApp.DB;

namespace ChatApp.Hubs
{
    public class AuthHub : Hub
    {
        public async Task Check(string userName)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var dbContent = httpContext.GetDbContent();
                if (dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName) != null)
                {
                    await Clients.Caller.SendAsync("Result", "exists");
                }
                else await Clients.Caller.SendAsync("Result", "available");
            }
            catch (Exception e) { await Clients.Caller.SendAsync("result", e.Message); }
        }

        public async Task CheckChange(string userName)
        {
            var httpContext = Context.GetHttpContext();
            var prevUserName = httpContext.User.Identity.Name;

            bool condition1 = prevUserName != userName;
            bool condition2 =  httpContext.GetDbContent().RegularUsers.FirstOrDefault(u => u.UserName == userName) == null;

            if (!condition1) await Clients.Caller.SendAsync("Result", "same");
            else if (!condition2) await Clients.Caller.SendAsync("Result", "exists");
            else await Clients.Caller.SendAsync("Result", "ok");
        }
    }
}