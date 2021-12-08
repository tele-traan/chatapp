using System.Collections.Generic;
namespace ChatApp.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public List<RoomUser> Users { get; set; }
    }
}
