using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ChatApp.DB;
using ChatApp.Hubs;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ChatDbContext _context;
        private readonly IHubContext<RoomHub> _roomHub;
        public UsersRepository(ChatDbContext context, IHubContext<RoomHub> roomHub)
        {
            _context = context;
            _roomHub = roomHub;
        }
        public IEnumerable<User> GetAllUsers() => _context.Users.IgnoreAutoIncludes().AsNoTracking();
        public bool AddUser(User user)
        {
            if (_context.Users.FirstOrDefault(u => u.Equals(user)) != null) return false;
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }
        public bool RemoveUser(User user)
        {
            if (_context.Users.FirstOrDefault(u => u.Equals(user)) == null) return false;
            Start:
            foreach (Room room in user.RoomsCreated)
            {
                if (room.Admins.Count > 0)
                {
                    room.Creator = room.Admins[new System.Random().Next(0, room.Admins.Count)];
                }
                else
                {
                    var list = _context.RoomUsers.Where(u => u.Room.Equals(room)).Select(u => u.ConnectionId);
                    _roomHub.Clients.Clients(list).SendAsync("RoomDeleted");
                    _context.Rooms.Remove(room);
                    _context.SaveChanges();
                    goto Start;
                }
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
        public User GetUser(string userName)
        {
            if (!_context.Users.Any(u => u.UserName == userName)) return null;
            return _context.Users.Where(u => u.UserName == userName)
               .Include(u => u.GlobalChatUser)
               .Include(u => u.RoomUser)
               .Include(u => u.ManagedRooms)
               .Include(u => u.RoomsCreated)
               .Include(u => u.RoomsWhereIsBanned)
               .Include(u => u.BanInfos)
               .Single();
        }
        public User GetUser(string userName, string password)
        {
            if (!_context.Users.Any(u => u.UserName == userName && u.PasswordHash == password)) return null;
             return _context.Users.Where(u => u.UserName == userName && u.PasswordHash == password)
                .Include(u => u.GlobalChatUser)
                .Include(u => u.RoomUser)
                .Include(u => u.ManagedRooms)
                .Include(u => u.RoomsCreated)
                .Include(u => u.RoomsWhereIsBanned)
                .Include(u => u.BanInfos)
                .Single();
        }
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}