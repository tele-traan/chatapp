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

        public IEnumerable<RoomUser> GetAllUsers() => _context.RoomUsers.AsNoTracking();
        public RoomUser GetUser(int id) => _context.RoomUsers.FirstOrDefault(u => u.RoomUserId == id);
        public RoomUser GetUser(string userName) => _context.RoomUsers.FirstOrDefault(u => u.UserName == userName);
    }
}
