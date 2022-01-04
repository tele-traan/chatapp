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
namespace ChatApp.Hubs
{
    public class AuthHub : Hub
    {
        public async Task CheckAvailability(string userName)
        {
            try
            {
                this.GetServices(out HttpContext httpContext, out ChatDbContext dbContext);
                if (await dbContext.RegularUsers.FirstOrDefaultAsync(u => u.UserName == userName) != null)
                {
                    await Clients.Caller.SendAsync("Result", "exists");
                }
                else await Clients.Caller.SendAsync("Result", "available");
            }
            catch (Exception e) { await Clients.Caller.SendAsync("Result", e.Message); }
        }

        public async Task CheckChange(string userName)
        {
            this.GetServices(out HttpContext httpContext, out ChatDbContext dbContext);
            var prevUserName = httpContext.User.Identity.Name;

            bool condition1 = prevUserName != userName;
            bool condition2 = dbContext.RegularUsers.FirstOrDefault(u => u.UserName == userName) == null;

            if (!condition1) await Clients.Caller.SendAsync("Result", "same");
            else if (!condition2) await Clients.Caller.SendAsync("Result", "exists");
            else await Clients.Caller.SendAsync("Result", "success");
        }

        public async Task ChangeUsername(string newUsername, string password)
        {
            this.GetServices(out HttpContext httpContext, out ChatDbContext dbContext);
            string prevUserName = httpContext.User.Identity.Name;
            if (prevUserName == newUsername)
            {
                await Clients.Caller.SendAsync("ChangeUsernameResult", "failure");
                return;
            }
            var user = dbContext.RegularUsers.FirstOrDefault(u => u.UserName == prevUserName && u.Password == password);
            if (user != null)
            {
                user.UserName = newUsername;
                dbContext.SaveChanges();
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await this.Authenticate(newUsername);
                await Clients.Caller.SendAsync("ChangeUsernameResult", "success");
            }
            else await Clients.Caller.SendAsync("ChangeUsernameResult", "failure");
        }

        public async Task ChangePassword(string oldPassword, string newPassword)
        {
            if (oldPassword == newPassword)
            {
                await Clients.Caller.SendAsync("ChangePasswordResult", "failure");
                return;
            }
            this.GetServices(out HttpContext httpContext, out ChatDbContext dbContext);
            string userName = httpContext.User.Identity.Name;
            var user = dbContext.GetUser(userName, oldPassword);
            if (user != null)
            {
                user.Password = newPassword;
                dbContext.SaveChanges();
                await Clients.Caller.SendAsync("ChangePasswordResult", "success");
            }
            else await Clients.Caller.SendAsync("ChangePasswordResult", "failure");
        }
    }
}