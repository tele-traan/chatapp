using ChatApp.DB;
using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System.Linq;
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
        public static DBContent GetDbContent(this HttpContext context) => 
            context.RequestServices.GetRequiredService<DBContent>();

        /// <summary>
        /// Gets the list containing connection IDs of users who should get the notifications from hub invocations
        /// </summary>
        /// <param name="hub">The hub that provides its HubCallerContext</param>
        /// <param name="roomName">Name of the specific room which contains all the users who should get the notifications</param>
        /// <returns>The list of users connection IDs</returns>
        public static List<string> GetIds(this Hub hub, string roomName)
        {
            List<string> list = new();
            hub.GetServices(out HttpContext context, out DBContent dbContent);
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
        public static List<string> GetIds(this RoomController controller, string roomName)
        {
            List<string> list = new();
            var context = controller.HttpContext;
            var dbContent = context.GetDbContent();
            var room = dbContent.Rooms.Include(r=>r.Users).FirstOrDefault(r => r.Name == roomName);
            list = room.Users.Select(u => u.ConnectionId).ToList();
            return list;
        }
        public static void GetServices(this Hub hub, out HttpContext httpContext, out DBContent dbContent)
        {
            httpContext = hub.Context.GetHttpContext();
            dbContent = httpContext.GetDbContent();
        }
    }
}
