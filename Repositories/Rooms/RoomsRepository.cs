using System.Linq;
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

        public Room GetRoom(string roomName) => _context.Rooms.FirstOrDefault(r => r.Name == roomName);
        
        public bool AddRoom(Room room)
        {
            if (_context.Rooms.FirstOrDefault(r => r.Equals(room)) != null) return false;
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return true;
        }
        public bool RemoveRoom(Room room)
        {
            if (_context.Rooms.FirstOrDefault(r => r.Equals(room)) == null) return false;
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
