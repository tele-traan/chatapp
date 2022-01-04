using ChatApp.DB;
using ChatApp.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatApp.Util
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the DBContent from HTTP context services
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>DBContent object for interacting with database</returns>
        public static ChatDbContext GetDbContext(this HttpContext context) => 
            context.RequestServices.GetRequiredService<ChatDbContext>();

        /// <summary>
        /// Gets the list containing connection IDs of users who should get the notifications from hub invocations
        /// </summary>
        /// <param name="hub">The hub that provides its HubCallerContext</param>
        /// <param name="roomName">Name of the specific room which contains all the users who should get the notifications</param>
        /// <returns>The list of users connection IDs</returns>
        public static List<string> GetIds(this Hub hub, string roomName)
        {
            List<string> list = new();
            hub.GetServices(out HttpContext context, out ChatDbContext dbContent);
            var room = dbContent.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == roomName);
            list = dbContent.RoomUsers.Where(u => u.Room.Name == roomName).Select(u => u.ConnectionId).ToList();
            return list;
        }
        /// <summary>
        /// Gets the list containing connection IDs of users who should get the notifications from hub invocations
        /// </summary>
        /// <param name="controller">The controller that provides its HttpContext</param>
        /// <param name="roomName">Name of the specific room which contains all the users who should get the notifications</param>
        /// <returns>The list of users connection IDs</returns>
        public static List<string> GetIds(this Microsoft.AspNetCore.Mvc.Controller controller, string roomName)
        {
            List<string> list = new();
            var context = controller.HttpContext;
            var dbContent = context.GetDbContent();
            var room = dbContent.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == roomName);
            list = room.Users.Select(u => u.ConnectionId).ToList();
            return list;
        }
        /// <summary>
        /// Handy method for getting HTTP context and database context in SignalR hub
        /// </summary>
        /// <param name="hub">Source hub, provider of HubContext</param>
        /// <param name="httpContext">The HttpContext to return</param>
        /// <param name="dbContent">The DbContext to return</param>
        public static void GetServices(this Hub hub, out HttpContext httpContext, out ChatDbContext dbContent)
        {
            httpContext = hub.Context.GetHttpContext();
            dbContent = httpContext.GetDbContent();
        }

        public static Task Authenticate(this Hub hub, string userName)
        {
            hub.GetServices(out HttpContext httpContext, out ChatDbContext content);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return httpContext.
                SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public static RegularUser GetUser(this ChatDbContext dbContext, string userName)
        => dbContext.RegularUsers.FirstOrDefault(u => u.UserName == userName);

        public static RegularUser GetUser(this ChatDbContext dbContext, string userName, string password)
        => dbContext.RegularUsers.FirstOrDefault(u => u.UserName == userName && u.Password == password);
    }
}
