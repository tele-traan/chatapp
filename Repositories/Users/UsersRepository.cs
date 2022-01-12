using System.Linq;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ChatDbContext _context;
        public UsersRepository(ChatDbContext context)
        {
            _context = context;
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
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
        public User GetUser(string userName) => _context.Users.FirstOrDefault(u => u.UserName == userName);
        public User GetUser(string userName, string password)
            => _context.Users.FirstOrDefault(u => u.UserName == userName && u.PasswordHash == password);
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
