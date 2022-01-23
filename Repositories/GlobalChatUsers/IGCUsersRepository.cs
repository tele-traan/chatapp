using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{    
    public interface IGCUsersRepository
    {
        IEnumerable<GlobalChatUser> GetAllUsers();
        GlobalChatUser GetUser(int id);
        GlobalChatUser GetUser(string userName);
        void RemoveUser(GlobalChatUser user);
    }
}
