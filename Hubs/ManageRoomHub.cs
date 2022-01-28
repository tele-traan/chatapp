using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using ChatApp.Util;
using ChatApp.Models;
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
            string roomName = httpContext.Session.GetString("currentlymanagedroom");
            Room room = roomsRepo.GetRoom(roomName);
            if (room is not null)
            {
                if (room.ContainsAdmin(httpContext.User.Identity.Name))
                {
                    await base.OnConnectedAsync();
                }
                else httpContext.Abort();
            }
            else httpContext.Abort();
        }
        public async Task MakeRoomPrivate(string roomPassword)
        {
            var httpContext = Context.GetHttpContext();

            var roomsRepo = this.GetService<IRoomsRepository>();
            string roomName = httpContext.Session.GetString("currentlymanagedroom");
            Room room = roomsRepo.GetRoom(roomName);
            if (room.ContainsAdmin(httpContext.User.Identity.Name))
            {
                if (!new Regex(@"(?!\s)(?=.*[0-9])(?=.*([a-zA-Z]|[а-яА-Я])).{6,30}")
                    .IsMatch(roomPassword))
                {
                    await Clients.Caller.SendAsync("MakePrivateResult", "conditionsnotpassed");
                    return;
                }
                room.IsPrivate = true;
                room.PasswordHash = roomPassword;
                roomsRepo.UpdateRoom(room);
                await Clients.Caller.SendAsync("MakePrivateResult", "success");
            }
            else httpContext.Abort();
        }
        public async Task MakeRoomUnprivate()
        {
            var httpContext = Context.GetHttpContext();

            var roomsRepo = this.GetService<IRoomsRepository>();
            string roomName = httpContext.Session.GetString("currentlymanagedroom");
            Room room = roomsRepo.GetRoom(roomName);
            if (room.ContainsAdmin(httpContext.User.Identity.Name))
            {
                room.IsPrivate = false;
                roomsRepo.UpdateRoom(room);
                await Clients.Caller.SendAsync("MakeUnprivateResult", "success");
            }
            else httpContext.Abort();
        }

        public async Task ChangeRoomName(string newName)
        {
            var httpContext = Context.GetHttpContext();

            var roomsRepo = this.GetService<IRoomsRepository>();
            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            if (room.ContainsAdmin(httpContext.User.Identity.Name))
            {
                if(newName == room.Name)
                {
                    await Clients.Caller.SendAsync("ChangeRoomNameResult", "same", "");
                    return;
                }
                room.Name = newName;
                httpContext.Session.SetString("currentlymanagedroom", newName);
                roomsRepo.UpdateRoom(room);
                await Clients.Caller.SendAsync("ChangeRoomNameResult", "success", newName);
            }
            else httpContext.Abort();
        }
        
        public async Task ChangeRoomPassword(string newPassword)
        {
            var httpContext = Context.GetHttpContext();

            var roomsRepo = this.GetService<IRoomsRepository>();
            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            if (room.ContainsAdmin(httpContext.User.Identity.Name))
            {
                if(this.GetHash(newPassword, room.Salt) == room.PasswordHash)
                {
                    await Clients.Caller.SendAsync("ChangeRoomPasswordResult", "same");
                    return;
                }
                if(!new Regex(@"(?!\s)(?=.*[0-9])(?=.*([a-zA-Z]|[а-яА-Я])).{6,30}")
                    .IsMatch(newPassword))
                {
                    await Clients.Caller.SendAsync("ChangeRoomPasswordResult", "conditionsnotpassed");
                    return;
                }
                room.PasswordHash = newPassword;
                roomsRepo.UpdateRoom(room);
                await Clients.Caller.SendAsync("ChangeRoomPasswordResult", "success");
            }
            else httpContext.Abort();
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
            if (room.Creator.Equals(usersRepo.GetUser(adminName))
                && room.ContainsUser(userName) 
                && !room.ContainsAdmin(userName))
            {
                user.RoomUser.IsAdmin = true;
                user.ManagedRooms.Add(room);
                usersRepo.UpdateUser(user);
                await roomHubContext.Clients.Client(user.RoomUser.ConnectionId)
                    .SendAsync("SystemMessage", $"Создатель {adminName} выдал вам права администратора", "lightgreen");
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
            if (room.Creator.Equals(usersRepo.GetUser(adminName))
                && room.ContainsUser(userName)
                && room.ContainsAdmin(userName))
            {
                user.RoomUser.IsAdmin = false;
                bool isRemoved = user.ManagedRooms.Remove(room);
                if (isRemoved)
                {
                    await Clients.Caller.SendAsync("DeopResult", "success", userName);
                    await roomHubContext.Clients.Client(user.RoomUser.ConnectionId)
                        .SendAsync("SystemMessage", $"Создатель {adminName} отнял у вас права администратора", "red");
                    usersRepo.UpdateUser(user);
                }
                else await Clients.Caller.SendAsync("DeopResult", "failure", "");
            }
            await Clients.Caller.SendAsync("DeopResult", "failure", userName);

        }
        public async Task Kick(string userName)
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
                await roomHubContext.Clients.Client(user.RoomUser.ConnectionId)
                    .SendAsync("UserKicked", adminName);
                roomUsersRepo.RemoveUser(user.RoomUser);
                await Clients.Caller.SendAsync("KickResult", "success", userName);
            }
            else await Clients.Caller.SendAsync("KickResult", "failure", "");
        }
        public async Task Ban(string userName, string reason, int days)
        {
                var httpContext = Context.GetHttpContext();
                var roomHubContext = this.GetService<IHubContext<RoomHub>>();

                var usersRepo = this.GetService<IUsersRepository>();
                var roomsRepo = this.GetService<IRoomsRepository>();
                var roomUsersRepo = this.GetService<IRoomUsersRepository>();
                var bansRepo = this.GetService<IBanInfoRepository>();

                var roomName = httpContext.Session.GetString("currentlymanagedroom");
                var room = roomsRepo.GetRoom(roomName);

                string adminName = httpContext.User.Identity.Name;
                var user = usersRepo.GetUser(userName);
                if (room.ContainsAdmin(adminName)
                    && room.BannedUsers.FirstOrDefault(u => u.Equals(user)) == null
                    && days > 0
                    && !room.Creator.Equals(user))
                {
                DateTime until = DateTime.Now.AddDays(days);

                if (string.IsNullOrEmpty(reason)) reason = "Без причины.";
                string connectionId = user.RoomUser.ConnectionId;

                await Clients.Caller.SendAsync("BanResult", "success", userName);

                bansRepo.AddBanInfo(until, reason, adminName, user, room);
                room.BannedUsers.Add(user);
                roomUsersRepo.RemoveUser(user.RoomUser);
                usersRepo.UpdateUser(user);
                roomsRepo.UpdateRoom(room);

                await roomHubContext.Clients.Client(connectionId)
                .SendAsync("UserBanned", adminName, reason, $"{until.ToShortDateString()} {until.ToShortTimeString()}");
            }
            else await Clients.Caller.SendAsync("BanResult", "failure", "");
        }
        public async Task Unban(string userName)
        {
            var httpContext = Context.GetHttpContext();
            var roomHubContext = this.GetService<IHubContext<RoomHub>>();

            var usersRepo = this.GetService<IUsersRepository>();
            var roomsRepo = this.GetService<IRoomsRepository>();
            var bansRepo = this.GetService<IBanInfoRepository>();

            var roomName = httpContext.Session.GetString("currentlymanagedroom");
            var room = roomsRepo.GetRoom(roomName);

            string adminName = httpContext.User.Identity.Name;
            var user = usersRepo.GetUser(userName);

            if (room is not null)
            {
                if (room.ContainsAdmin(adminName) && room.BannedUsers.FirstOrDefault(u => u.Equals(user)) is not null)
                {
                    var banInfo = user.BanInfos.FirstOrDefault(b => b.User.Equals(user));
                    bansRepo.RemoveBanInfo(banInfo);
                    room.BannedUsers.Remove(user);
                    roomsRepo.UpdateRoom(room);
                    await Clients.Caller.SendAsync("UnbanResult", "success", userName);
                }
                else await Clients.Caller.SendAsync("UnbanResult", "failure", "");
            } else await Clients.Caller.SendAsync("UnbanResult", "failure", "");
        }
    }
}