using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class RoomUsersRepository : IRoomUsersRepository
    {
        private readonly ChatDbContext _context;
        public RoomUsersRepository(ChatDbContext context)
        {
            _context = context;
        }

        public RoomUser GetUser(int id)
        {
            if (!_context.RoomUsers.Any(u => u.RoomUserId == id) ) return null;
            return _context.RoomUsers
                .Where(u => u.RoomUserId == id)
                .Include(u => u.Room)
                .Include(u => u.User)
                .Single();
        }
        public RoomUser GetUser(string userName)
        {
            if (!_context.RoomUsers.Any(u=>u.UserName==userName)) return null;
            return _context.RoomUsers
                .Where(u => u.UserName == userName)
                .Include(u => u.Room)
                .Include(u => u.User)
                .Single();
        }
        public void RemoveUser(RoomUser user)
        {
            _context.RoomUsers.Remove(user);
            _context.SaveChanges();
        }
    }
}
