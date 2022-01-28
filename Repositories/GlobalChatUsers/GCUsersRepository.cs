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
        
        public IEnumerable<GlobalChatUser> GetAllUsers() => _context.GlobalChatUsers.IgnoreAutoIncludes().AsNoTracking();
        public GlobalChatUser GetUser(int id)
        {
            if (!_context.GlobalChatUsers.Any(gcu => gcu.GlobalChatUserId == id)) return null; 
            return _context.GlobalChatUsers.IgnoreAutoIncludes()
                .Where(gcu => gcu.GlobalChatUserId == id)
                .Include(gcu => gcu.User)
                .Single();
        }
        public GlobalChatUser GetUser(string userName)
        {
            if (!_context.GlobalChatUsers.Any(gcu => gcu.UserName == userName)) return null;
            return _context.GlobalChatUsers.IgnoreAutoIncludes()
                .Where(gcu => gcu.UserName == userName)
                .Include(gcu => gcu.User)
                .Single();
        }
        public void RemoveUser(GlobalChatUser user)
        {
            _context.GlobalChatUsers.Remove(user);
            _context.SaveChanges();
        }
    }
}
