using System.Threading.Tasks;
using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IRoomsRepository
    {
        IEnumerable<Room> GetAllRooms();
        Room GetRoom(int roomId);
        Room GetRoom(string roomName);
        bool AddRoom(Room room);
        bool RemoveRoom(Room room);
        void UpdateRoom(Room room);
    }
}