using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IUsersRepository
    {
        IEnumerable<User> GetAllUsers();
        bool AddUser(User user);
        bool RemoveUser(User user);
        User GetUser(string userName);
        User GetUser(string userName, string password);
        void UpdateUser(User user);
    }
}