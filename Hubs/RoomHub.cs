﻿using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using ChatApp.Util;
using ChatApp.Models;
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

                var room = roomsRepo.GetRoom(roomName);
                var time = DateTime.Now;
                Message msg = new()
                { Text = $"Пользователь {userName} подключился к комнате", DateTime = time, SenderName=null, BgColor = "lightgreen" };
                if (user.RoomUser.IsAdmin) msg.Text = $"Админ {userName} подключился к комнате";
                this.AddLastMessage(room, msg);
                var connectionIds = this.GetIds(roomName);
                await Clients.Clients(connectionIds).SendAsync("MemberJoined", userName, user.RoomUser.IsAdmin, time.ToShortTimeString());
            }
            else
            {
                await Clients.Caller.SendAsync("ErrorLogging",
                    "Ошибка при входе в комнату. Попробуйте ещё раз");
            }
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var roomUsersRepo = this.GetService<IRoomUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();
            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = roomUsersRepo.GetUser(userName);
            if (user is not null) 
            {
                string roomName = user.Room.Name;
                var room = roomsRepo.GetRoom(roomName);
                roomUsersRepo.RemoveUser(user);

                Message msg = new() 
                { Text = $"Пользователь {userName} покинул комнату", DateTime = DateTime.Now, SenderName=null, BgColor = "crimson" };
                if (user.IsAdmin) msg.Text = $"Админ {userName} покинул комнату";
                this.AddLastMessage(room, msg);

                var connectionIds = this.GetIds(roomName);
                await Clients.Clients(connectionIds).SendAsync("MemberLeft", userName, user.IsAdmin, DateTime.Now.ToShortTimeString());
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            var usersRepo = this.GetService<IRoomUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();

            var httpContext = Context.GetHttpContext();
            string userName = httpContext.User.Identity.Name;
            var user = usersRepo.GetUser(userName);
            if (user is null)
            {
                Context.Abort();
                return;
            }
            string roomName = user.Room.Name;
            var room = roomsRepo.GetRoom(roomName);
            var time = DateTime.Now;
            this.AddLastMessage(room, new() { Text = message, SenderName = userName, DateTime = time, BgColor = "transparent" });

            var connectionIds = this.GetIds(roomName);
            await Clients.Clients(connectionIds).SendAsync("NewMessage", time.ToShortTimeString(), userName, message.Trim());
        }
    }
}