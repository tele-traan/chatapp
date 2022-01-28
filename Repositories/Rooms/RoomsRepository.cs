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

        public Room GetRoom(int roomId)
        {
            if (!_context.Rooms.Any(r => r.RoomId == roomId)) return null;
            return _context.Rooms
                .Where(r => r.RoomId == roomId)
                .Include(r => r.RoomUsers)
                .Include(r => r.Admins)
                .Include(r => r.BannedUsers)
                .Include(r => r.Creator)
                .Include(r => r.BanInfos)
                .Include(r => r.LastMessages)
                .Single();
        }
        public Room GetRoom(string roomName)
        {
            if (!_context.Rooms.Any(r => r.Name== roomName)) return null;
            return _context.Rooms
                .Where(r => r.Name == roomName)
                .Include(r => r.RoomUsers)
                .Include(r => r.Admins)
                .Include(r => r.BannedUsers)
                .Include(r => r.Creator)
                .Include(r => r.BanInfos)
                .Include(r => r.LastMessages)
                .Single();
        }
        public bool AddRoom(Room room)
        {
            if (_context.Rooms.FirstOrDefault(r => r.Equals(room)) is not null) return false;
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return true;
        }
        public bool RemoveRoom(Room room)
        {
            if (_context.Rooms.FirstOrDefault(r => r.Equals(room)) is null) return false;
            _context.Rooms.Remove(room);
            _context.SaveChanges();
            return true;
        }
        public void UpdateRoom(Room room)
        {
            _context.Rooms.Update(room);
            _context.SaveChanges();
        }
    }
}
