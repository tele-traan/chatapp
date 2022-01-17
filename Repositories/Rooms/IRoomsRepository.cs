using System.Threading.Tasks;
using System.Collections.Generic;

using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IRoomsRepository
    {
        IEnumerable<Room> GetAllRooms();
        Task<Room> GetRoomAsync(int roomId);
        Task<Room> GetRoomAsync(string roomName);
        Task<bool> AddRoomAsync(Room room);
        Task<bool> RemoveRoomAsync(Room room);
        void UpdateRoom(Room room);
    }
}
