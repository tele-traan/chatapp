using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class GCUsersRepository : IGCUsersRepository
    {
        private readonly ChatDbContext _context;
        public GCUsersRepository(ChatDbContext context)
        {
            _context = context;
        }

        public IEnumerable<GlobalChatUser> GetAllUsers() => _context.GlobalChatUsers.AsNoTracking();

        public GlobalChatUser GetUser(int id) => _context.GlobalChatUsers.FirstOrDefault(u => u.Id == id);
        public GlobalChatUser GetUser(string userName) => _context.GlobalChatUsers.FirstOrDefault(u => u.UserName == userName);
    }
}
