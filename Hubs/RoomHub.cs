using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using ChatApp.Util;
using ChatApp.DB;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatApp.Hubs
{
    public class RoomHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.User.Identity.Name;

            var user = dbContent.RegularUsers
                .Include(ru=>ru.RoomUser)
                .ThenInclude(r=>r.Room)
                .FirstOrDefault(u => u.UserName == userName);

            string roomName = user.RoomUser.Room.Name;
            var room = dbContent.Rooms.Include(r => r.Users).FirstOrDefault(r => r.Name == roomName);
            await Clients.All.SendAsync("NewMessage", $"{user.RoomUser.ConnectionId}, {Context.ConnectionId}");

            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            await base.OnConnectedAsync();
            if (condition)
            {
                user.RoomUser.ConnectionId = Context.ConnectionId;
                await dbContent.SaveChangesAsync();
                await Clients.Clients(this.GetIds(roomName)).SendAsync("MemberJoined", userName);
            }
            else
            {
                await Clients.Caller.SendAsync("ErrorLogging", "Ошибка при входе в комнату. Попробуйте ещё раз");
            }
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.User.Identity.Name;
            var user = dbContent.RegularUsers.FirstOrDefault(u => u.UserName == userName);

            string roomName = user.RoomUser.Room.Name;
            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            if (condition)
            {
                user.RoomUser = null;
                dbContent.SaveChanges();
                await Clients.Clients(this.GetIds(roomName)).SendAsync("MemberLeft", userName);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            string userName = httpContext.User.Identity.Name;
            var user = dbContent.RegularUsers.Include(r=>r.RoomUser).ThenInclude(ru=>ru.Room).FirstOrDefault(u => u.UserName == userName);
            string roomName = user.RoomUser.Room.Name;
            string time = DateTime.Now.ToShortTimeString();
            await Clients.Clients(this.GetIds(roomName)).SendAsync("NewMessage", time, userName, message.Trim());
        }
        [Authorize(Roles ="Admin")]
        public async Task RoomDeleted()
        {
            this.GetServices(out HttpContext httpContext, out DBContent dbContent);
            bool condition = httpContext.User.IsInRole("Admin");
            if (condition)
            {
                string userName = httpContext.User.Identity.Name;
                var user = await dbContent.RoomUsers.FirstOrDefaultAsync(u => u.UserName == userName);

                var rooms = dbContent.Rooms;
                var room = user.Room;

                await Clients.Clients(this.GetIds(room.Name)).SendAsync("RoomDeleted");

                rooms.Remove(room);
                dbContent.SaveChanges();
            }
        }
    }
}