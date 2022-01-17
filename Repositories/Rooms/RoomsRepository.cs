using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using ChatApp.DB;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class RoomsRepository : IRoomsRepository
    {
        private readonly ChatDbContext _context;
        public RoomsRepository(ChatDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Room> GetAllRooms() => _context.Rooms.AsNoTracking();

        public async Task<Room> GetRoomAsync(int roomId) => await _context.Rooms
            .Include(r => r.RoomUsers)
            .Include(r => r.Admins)
            .Include(r => r.BannedUsers)
            .Include(r => r.Creator)
            .Include(r => r.BanInfos)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);
        public async Task<Room> GetRoomAsync(string roomName) => await _context.Rooms
            .Include(r => r.RoomUsers)
            .Include(r => r.Admins)
            .Include(r => r.BannedUsers)
            .Include(r => r.Creator)
            .Include(r => r.BanInfos)
            .FirstOrDefaultAsync(r => r.Name == roomName);
        public async Task<bool> AddRoomAsync(Room room)
        {
            if (_context.Rooms.FirstOrDefault(r => r.Equals(room)) is not null) return false;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveRoomAsync(Room room)
        {
            if (_context.Rooms.FirstOrDefault(r => r.Equals(room)) is null) return false;
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }
        public void UpdateRoom(Room room)
        {
            _context.Rooms.Update(room);
            _context.SaveChanges();
        }
    }
}
