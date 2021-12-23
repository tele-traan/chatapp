using System.Collections.Generic;
namespace ChatApp.Models
{
    public class RoomViewModel : BaseViewModel
    {
        public string RoomName { get; set; }
        public string Type { get; set; }
        public List<Room> Rooms { get; set; }
        public List<RoomUser> UsersInRoom { get; set; }
    }
}
