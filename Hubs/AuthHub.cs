using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using ChatApp.DB;
using ChatApp.Util;
using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Hubs
{
    public class AuthHub : Hub
    {
        public async Task CheckAvailability(string userName)
        {
            try
            {
                var usersRepo = this.GetService<IUsersRepository>();
                if (usersRepo.GetUser(userName) != null)
                {
                    await Clients.Caller.SendAsync("Result", "exists");
                }
                else await Clients.Caller.SendAsync("Result", "available");
            }
            catch (Exception e) { await Clients.Caller.SendAsync("Result", e.Message); }
        }
        public async Task CheckChange(string userName)
        {
            var usersRepo = this.GetService<IUsersRepository>();
            var prevUserName = Context.GetHttpContext().User.Identity.Name;

            bool condition1 = prevUserName != userName;
            bool condition2 = usersRepo.GetUser(userName) == null;

            if (!condition1) await Clients.Caller.SendAsync("Result", "same");
            else if (!condition2) await Clients.Caller.SendAsync("Result", "exists");
            else await Clients.Caller.SendAsync("Result", "success");
        }
    }
}