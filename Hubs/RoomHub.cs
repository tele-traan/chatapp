using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using ChatApp.DB;
using ChatApp.Util;
using ChatApp.Repositories;

namespace ChatApp.Hubs
{
    [Authorize]
    public class RoomHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            var usersRepo = this.GetService<IUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();

            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = usersRepo.GetUser(userName);

            string roomName = user.RoomUser.Room.Name;

            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            await base.OnConnectedAsync();
            if (condition)
            {
                user.RoomUser.ConnectionId = Context.ConnectionId;
                usersRepo.UpdateUser(user);
                await Clients.Clients(this.GetIds(roomName)).SendAsync("MemberJoined", userName);
            }
            else
            {
                await Clients.Caller.SendAsync("ErrorLogging", "Ошибка при входе в комнату. Попробуйте ещё раз");
            }
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var usersRepo = this.GetService<IUsersRepository>();

            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = usersRepo.GetUser(userName);

            string roomName = user.RoomUser.Room.Name;
            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            if (condition)
            {
                user.RoomUser = null;
                usersRepo.UpdateUser(user);
                await Clients.Clients(this.GetIds(roomName)).SendAsync("MemberLeft", userName);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            var usersRepo = this.GetService<IUsersRepository>();
            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = usersRepo.GetUser(userName);
            string roomName = user.RoomUser.Room.Name;
            string time = DateTime.Now.ToShortTimeString();
            await Clients.Clients(this.GetIds(roomName)).SendAsync("NewMessage", time, userName, message.Trim());
        }
        public async Task RoomDeleted()
        {
            var roomUsersRepo = this.GetService<IRoomUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();

            string userName = Context.GetHttpContext().User.Identity.Name;
            var roomUser = roomUsersRepo.GetUser(userName);
            var room = roomUser.Room;

            bool condition = room.Admins.FirstOrDefault(u => u.Equals(roomUser.User))!=null;
            if (condition)
            {
                await Clients.Clients(this.GetIds(room.Name)).SendAsync("RoomDeleted");
                roomsRepo.RemoveRoom(room);
            }
        }
    }
}