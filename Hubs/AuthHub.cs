using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
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
    }
}