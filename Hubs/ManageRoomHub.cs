using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using ChatApp.Util;
using ChatApp.Repositories;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ManageRoomHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var httpContext = Context.GetHttpContext();

            var roomsRepo = this.GetService<IRoomsRepository>();
            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);
            await Clients.All.SendAsync("KickResult", "success", $"room is null-${room is null}");
            if (room is not null)
            {
                if (room.ContainsAdmin(httpContext.User.Identity.Name))
                {
                    await base.OnConnectedAsync();
                }
                else Context.GetHttpContext().Abort();
            }
            else Context.GetHttpContext().Abort();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Context.GetHttpContext().Session.SetString("currentlymanagedroom", "");
            await base.OnDisconnectedAsync(exception);
        }
        public async Task Op(string userName)
        {
            var httpContext = Context.GetHttpContext();
            var roomHubContext = this.GetService<IHubContext<RoomHub>>();

            var roomsRepo = this.GetService<IRoomsRepository>();
            var usersRepo = this.GetService<IUsersRepository>();

            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            string adminName = httpContext.User.Identity.Name; 
            var user = usersRepo.GetUser(userName);
            if (room.ContainsAdmin(adminName) && room.ContainsUser(userName) && !room.ContainsAdmin(userName))
            {
                user.RoomUser.IsAdmin = true;
                user.ManagedRooms.Add(room);
                usersRepo.UpdateUser(user);
                await roomHubContext.Clients.Client(user.RoomUser.ConnectionId).SendAsync("UserOpped", adminName);
                await Clients.Caller.SendAsync("OpResult", "success", userName);
            }
            else await Clients.Caller.SendAsync("OpResult", "failure", "");

        }
        public async Task Deop(string userName)
        {
            var httpContext = Context.GetHttpContext();
            var roomHubContext = this.GetService<IHubContext<RoomHub>>();

            var usersRepo = this.GetService<IUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();

            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            string adminName = httpContext.User.Identity.Name;
            var user = usersRepo.GetUser(userName);
            if (room.ContainsAdmin(adminName) && room.ContainsUser(userName) && room.ContainsAdmin(userName))
            {
                user.RoomUser.IsAdmin = false;
                bool isRemoved = user.ManagedRooms.Remove(room);
                if (isRemoved)
                {
                    await Clients.Caller.SendAsync("OpResult", "success", userName);
                    await roomHubContext.Clients.Client(user.RoomUser.ConnectionId).SendAsync("UserDeopped", adminName);
                    usersRepo.UpdateUser(user);
                }
                else await Clients.Caller.SendAsync("OpResult", "failure", "");
            }
            await Clients.Caller.SendAsync("OpResult", "failure", userName);

        }
        public async Task Kick(string userName, string reason)
        {
            var httpContext = Context.GetHttpContext();
            var roomHubContext = this.GetService<IHubContext<RoomHub>>();

            var usersRepo = this.GetService<IUsersRepository>();
            var roomUsersRepo = this.GetService<IRoomUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();

            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            var adminName = httpContext.User.Identity.Name;
            var user = usersRepo.GetUser(userName);
            if (room.ContainsAdmin(adminName) && room.ContainsUser(userName))
            {
                if (string.IsNullOrEmpty(reason)) reason = "Без причины.";
                await roomHubContext.Clients.Client(user.RoomUser.ConnectionId).SendAsync("UserKicked", adminName, reason);
                roomUsersRepo.RemoveUser(user.RoomUser);
                await Clients.Caller.SendAsync("KickResult", "success", userName);
            }
            else await Clients.Caller.SendAsync("KickResult", "failure", "");
        }
        public async Task Ban(string userName, string reason, int days)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var roomHubContext = this.GetService<IHubContext<RoomHub>>();

                var usersRepo = this.GetService<IUsersRepository>();
                var roomsRepo = this.GetService<IRoomsRepository>();
                var roomUsersRepo = this.GetService<IRoomUsersRepository>();
                var bansRepo = this.GetService<IBanInfoRepository>();

                var roomName = httpContext.Session.GetString("currentlymanagedroom");
                var room = roomsRepo.GetRoom(roomName);

                var adminName = httpContext.User.Identity.Name;
                var user = usersRepo.GetUser(userName);
                
                if (room.ContainsAdmin(adminName)
                    && room.BannedUsers.FirstOrDefault(u => u.Equals(user)) == null
                    && days > 0
                    && !room.Creator.Equals(user))
                {
                    DateTime until = DateTime.Now.AddDays(days);
                    if (string.IsNullOrEmpty(reason)) reason = "Без причины.";

                    await roomHubContext.Clients.Client(user.RoomUser.ConnectionId)
                        .SendAsync("UserBanned", adminName, reason,
                        $"{until.ToShortDateString()} {until.ToShortTimeString()}");
                    await Clients.Caller.SendAsync("BanResult", "success", userName);
                    bansRepo.AddBanInfo(until, reason, adminName, user, room);
                    room.BannedUsers.Add(user);
                    roomUsersRepo.RemoveUser(user.RoomUser);
                    usersRepo.UpdateUser(user);
                    roomsRepo.UpdateRoom(room);


                    await Clients.Caller.SendAsync("BanResult", "success", userName);
                }
                else await Clients.Caller.SendAsync("BanResult", "failure", "");
            } catch (Exception e)
            {
                await Clients.Caller.SendAsync("BanResult", "success", e.Message);
            }
        }
        public async Task Unban(string userName)
        {
            var httpContext = Context.GetHttpContext();
            var roomHubContext = this.GetService<IHubContext<RoomHub>>();

            var usersRepo = this.GetService<IUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();

            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            string adminName = httpContext.User.Identity.Name;
            var user = usersRepo.GetUser(userName);
            if (room.ContainsAdmin(adminName) && room.BannedUsers.FirstOrDefault(u=>u.Equals(user))is not null)
            {
                bool isRemoved = room.BannedUsers.Remove(user);
                if (isRemoved)
                {
                    roomsRepo.UpdateRoom(room);
                    await Clients.Caller.SendAsync("UnanResult", "success", userName);
                }
                else await Clients.Caller.SendAsync("UnbanResult", "success", "");
            }
            else await Clients.Caller.SendAsync("UnbanResult", "success", "");
        }
    }
}