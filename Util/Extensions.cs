using ChatApp.Models;
using ChatApp.DB;
using ChatApp.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using ChatApp.Controllers;
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
        public static List<string> GetIds(this RoomHub hub, string roomName)
        {
            List<string> list = new();
            var context = hub.Context.GetHttpContext();
            var dbContent = context.GetDbContent();
            var room = dbContent.Rooms.FirstOrDefault(r => r.Name == roomName);
            list = room.Users.Select(u => u.UserConnectionId).ToList();
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
            var room = dbContent.Rooms.FirstOrDefault(r => r.Name == roomName);
            list = room.Users.Select(u => u.UserConnectionId).ToList();
            return list;
        }
    }
}
