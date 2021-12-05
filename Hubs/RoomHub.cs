using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace ChatApp.Hubs
{
    public class RoomHub : Hub
    {
        public async Task MemberJoined(string roomName, string memberName)
        {
            List<string> excluded = this.GetExcluded(roomName);
            
            var httpContext = Context.GetHttpContext();
            var dbContent = httpContext.GetDbContent();
            var room = dbContent.Rooms.FirstOrDefault(r => r.Name == roomName);

            room.Users?.Add(new User { Username = memberName, UserConnectionId = Context.ConnectionId });
            dbContent.SaveChanges();

            string msg = $"Пользователь {memberName} подключился";
            await Clients.AllExcept(excluded).SendAsync("MemberJoined", msg);
            await Clients.All.SendAsync("MemberJoined", "KEKW");
        }
        public async Task MemberLeft(string roomName, string memberName)
        {
            var httpContext = GetHttpContextExtensions.GetHttpContext(Context);
            var dbContent = httpContext.GetDbContent();

            var room = dbContent.Rooms.FirstOrDefault(r => r.Name == roomName); 
            room.Users.Remove(room.Users.FirstOrDefault(u=>u.Username == memberName&&u.UserConnectionId==Context.ConnectionId));
            dbContent.SaveChanges();

            List<string> excluded = this.GetExcluded(roomName);
            await Clients.AllExcept(excluded).SendAsync("MemberLeft", memberName);
        }

        public async Task NewMessage(string roomName, string memberName, string message)
        {
            var time = DateTime.Now.ToShortTimeString()+ $" ConnectionId = {Context.ConnectionId}";
            List<string> excluded = this.GetExcluded(roomName);
            await Clients.AllExcept(excluded).SendAsync("NewMessage", time, memberName, message);
        }

        public async Task RoomDeleted(string roomName)
        {
            var httpContext = GetHttpContextExtensions.GetHttpContext(Context);
            var dbContent = httpContext.GetDbContent();

            var rooms = dbContent.Rooms;
            var room = rooms.FirstOrDefault(r => r.Name == roomName);

            List<string> excluded = this.GetExcluded(roomName);
            rooms.Remove(room);
            dbContent.SaveChanges();
            await Clients.AllExcept(excluded).SendAsync("RoomDeleted");
        }
    }
}