using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

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
            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = usersRepo.GetUser(userName);

            string roomName = user.RoomUser.Room.Name;

            bool condition = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(roomName);
            await base.OnConnectedAsync();
            if (condition)
            {
                user.RoomUser.ConnectionId = Context.ConnectionId;
                usersRepo.UpdateUser(user);
                var connectionIds = await this.GetIds(roomName);
                await Clients.Clients(connectionIds)
                    .SendAsync("MemberJoined", userName, user.RoomUser.IsAdmin);
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
            string userName = Context.GetHttpContext().User.Identity.Name;
            var user = roomUsersRepo.GetUser(userName);
            if (user is not null) 
            {
                string roomName = user.Room.Name;

                roomUsersRepo.RemoveUser(user);
                var connectionIds = await this.GetIds(roomName);
                await Clients.Clients(connectionIds)
                    .SendAsync("MemberLeft", userName, user.IsAdmin);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task NewMessage(string message)
        {
            var usersRepo = this.GetService<IUsersRepository>();
            var httpContext = Context.GetHttpContext();
            string userName = httpContext.User.Identity.Name;
            var user = usersRepo.GetUser(userName);
            if(user.RoomUser is null) Context.GetHttpContext().Abort();
            
            string roomName = user.RoomUser.Room.Name;
            string time = DateTime.Now.ToShortTimeString();
            var connectionIds = await this.GetIds(roomName);
            await Clients.Clients(connectionIds).SendAsync("NewMessage", time, userName, message.Trim());
        }
    }
}