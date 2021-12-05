using System.Collections.Generic;
namespace ChatApp.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; } = new();
        public List<User> Admins { get; set; } = new();
    }
}
