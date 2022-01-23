using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IRoomUsersRepository
    {
        IEnumerable<RoomUser> GetAllUsers();
        RoomUser GetUser(int id);
        RoomUser GetUser(string userName);
        void RemoveUser(RoomUser user);
    }
}